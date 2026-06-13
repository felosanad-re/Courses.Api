using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Analyzer;
using Courses.Core.ModelsDTO.ResponseDTO.Analyses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Services.Contract.AnalyticsServices;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Specifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Courses.Services.AnalyticsServices
{
    public class AnalyzeService : IAnalyzeService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<AnalyzeService> _logger;
        protected readonly IMapper _mapper;
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        public AnalyzeService(IUnitOfWork unitOfWork, ILogger<AnalyzeService> logger, ICurrentInstructorServices currentInstructorServices, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentInstructorServices = currentInstructorServices;
            _mapper = mapper;
        }

        #region Get Analyze Async
        public async Task<ApplicationServiceResult<InstructorAnalyticsDto>> GetAnalyzeAsync()
        {
            var userNotFoundMessage = "There is no instructor with this id";
            var succeededMessage = "this all Analytics for instructor";
            var loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                instructorId = await GetCurrentInstructor();
                if (instructorId is null)
                    return ApplicationServiceResult<InstructorAnalyticsDto>.Fail(userNotFoundMessage);

                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var coursesRepo = _unitOfWork.CreateRepository<Course>();
                var enrollmentSpec = new BaseSpecifications<Enrollment>(x => x.Course.InstructorId == instructorId);
                enrollmentSpec.Includes.Add(x => x.Course);

                var coursesSpec = new BaseSpecifications<Course>(x => x.InstructorId == instructorId);

                var courses = await coursesRepo.GetAllAsyncSpec(coursesSpec);
                if (!courses.Any())
                    return ApplicationServiceResult<InstructorAnalyticsDto>.Success(new InstructorAnalyticsDto
                    {
                        TopCourseRating = null,
                        TopCourseSelling = null,
                        AverageCourseRating = 0,
                        DraftCourses = 0,
                        PublishedCourses = 0,
                        TotalCourses = 0,
                        TotalEnrollments = 0,
                        TotalRevenue = 0,
                        TotalStudents = 0
                    }, succeededMessage);

                var enrollments = await enrollmentRepo.GetAllAsyncSpec(enrollmentSpec);

                // Get Top Best Selling Course (by revenue, then by enrollment count)
                var bestSellingCourseEnrollments = enrollments
                    .GroupBy(x => x.Course)
                    .Select(group => new
                    {
                        Course = group.Key,
                        Revenue = group.Where(x => x.IsPaid).Sum(x => x.Amount),
                        Enrollments = group.Count()
                    }).OrderByDescending(x => x.Revenue)
                      .ThenByDescending(x => x.Enrollments)
                      .FirstOrDefault();

                // Auto Mapping For Image Resolver
                CourseAnalyticDTO? bestSellingCourse = null;
                if(bestSellingCourseEnrollments != null)
                {
                    bestSellingCourse = _mapper.Map<CourseAnalyticDTO>(bestSellingCourseEnrollments.Course);
                    bestSellingCourse.Revenue = bestSellingCourseEnrollments.Revenue;
                    bestSellingCourse.Enrollments = bestSellingCourseEnrollments.Enrollments;
                }

                var data = new InstructorAnalyticsDto
                {
                    TotalCourses = courses.Count,
                    TotalEnrollments = enrollments.Count,
                    TotalStudents = enrollments.Select(x => x.StudentId).Distinct().Count(),
                    TotalRevenue = enrollments.Where(x => x.IsPaid).Sum(x => x.Amount),
                    TopCourseSelling = bestSellingCourse
                };

                return ApplicationServiceResult<InstructorAnalyticsDto>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Analytic For Instructor Id {instructorId}", instructorId);
                return ApplicationServiceResult<InstructorAnalyticsDto>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Instructor Charts Analytics
        public async Task<ApplicationServiceResult<IReadOnlyList<MonthlyAnalyticsDto>>> GetInstructorChartsAnalyticsAsync(ChartRequest req)
        {
            var userNotFoundMessage = "There is no instructor with this id";
            var succeededMessage = "this all Analytics Charts for instructor";
            var loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                instructorId = await GetCurrentInstructor();
                if (instructorId is null)
                    return ApplicationServiceResult<IReadOnlyList<MonthlyAnalyticsDto>>.Fail(userNotFoundMessage);

                // Get date range (default: last 12 months)
                var toDate = req.ToDate ?? DateTime.UtcNow;
                var fromDate = req.FromDate ?? DateTime.UtcNow.AddMonths(-12);
                var range = (toDate - fromDate).TotalDays;

                // Check In Range
                // 1. If FromDate > ToDate
                if(req.FromDate.HasValue && req.ToDate.HasValue && req.FromDate > req.ToDate)
                    return ApplicationServiceResult<IReadOnlyList<MonthlyAnalyticsDto>>.Fail("FromDate cannot be greater than ToDate");
                // Check If Data Too Large
                if((toDate - fromDate).TotalDays > 1500)
                    return ApplicationServiceResult<IReadOnlyList<MonthlyAnalyticsDto>>.Fail("Date range is too large.");

                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();

                var enrollmentSpec = BuildEnrollmentAnalyticsSpec(instructorId, req);

                var enrollments = await enrollmentRepo.GetAllAsyncSpec(enrollmentSpec);

                // Choose grouping strategy based on range duration
                List<MonthlyAnalyticsDto> allPeriods;
                if (range <= 30)
                    allPeriods = GroupByDay(enrollments, fromDate, toDate);
                else if (range <= 90)
                    allPeriods = GroupByWeek(enrollments, fromDate, toDate);
                else
                    allPeriods = GroupByMonth(enrollments, fromDate, toDate);

                return ApplicationServiceResult<IReadOnlyList<MonthlyAnalyticsDto>>.Success(allPeriods, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Analytic For Instructor Id {instructorId}", instructorId);
                return ApplicationServiceResult<IReadOnlyList<MonthlyAnalyticsDto>>.Fail(loggerError);
            }
        }
        #endregion

        #region Group By Day Helper
        /// <summary>
        /// Groups enrollments by day and normalizes: fills missing days with zero values.
        /// </summary>
        private List<MonthlyAnalyticsDto> GroupByDay(IReadOnlyList<Enrollment> enrollments, DateTime fromDate, DateTime toDate)
        {
            // 1. Aggregate actual enrollment data grouped by day
            var grouped = enrollments.GroupBy(x => x.CreatedAt.Date)
                .Select(group => new MonthlyAnalyticsDto
                {
                    MonthLabel = group.Key.ToString("yyyy-MM-dd"),
                    Month = group.Key.Month,
                    Years = group.Key.Year,
                    Earnings = group.Where(x => x.IsPaid).Sum(x => x.Amount),
                    Students = group.Select(x => x.StudentId).Distinct().Count()
                }).ToList();

            // 2. Normalize: ensure every day in the range has an entry (0 if no data)
            var dict = grouped.ToDictionary(x => x.MonthLabel);
            var result = new List<MonthlyAnalyticsDto>();
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                var label = date.ToString("yyyy-MM-dd");
                if (dict.TryGetValue(label, out var existing))
                    result.Add(existing);
                else
                    result.Add(new MonthlyAnalyticsDto
                    {
                        MonthLabel = label,
                        Month = date.Month,
                        Years = date.Year,
                        Earnings = 0,
                        Students = 0
                    });
            }
            return result;
        }
        #endregion

        #region Group By Week Helper
        /// <summary>
        /// Groups enrollments by ISO week and normalizes: fills missing weeks with zero values.
        /// </summary>
        private List<MonthlyAnalyticsDto> GroupByWeek(IReadOnlyList<Enrollment> enrollments, DateTime fromDate, DateTime toDate)
        {
            var cal = CultureInfo.CurrentCulture.Calendar;

            // 1. Aggregate actual enrollment data grouped by week
            var grouped = enrollments.GroupBy(x => new
            {
                x.CreatedAt.Year,
                Week = cal.GetWeekOfYear(x.CreatedAt, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
            }).Select(group => new MonthlyAnalyticsDto
            {
                MonthLabel = $"Week {group.Key.Week}",
                Earnings = group.Where(x => x.IsPaid).Sum(x => x.Amount),
                Students = group.Select(x => x.StudentId).Distinct().Count(),
                Month = group.Key.Week,
                Years = group.Key.Year
            }).ToList();

            // 2. Normalize: generate all weeks in range, fill missing with zeros
            var dict = grouped.ToDictionary(x => $"{x.Years}-W{x.Month}");
            var result = new List<MonthlyAnalyticsDto>();
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(7))
            {
                var weekNumber = cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var key = $"{date.Year}-W{weekNumber}";
                if (dict.TryGetValue(key, out var existing))
                    result.Add(existing);
                else
                {
                    var weekLabel = $"Week {weekNumber}";
                    // Avoid duplicate weeks (e.g., year boundary edge cases)
                    if (!result.Any(x => x.MonthLabel == weekLabel && x.Years == date.Year))
                        result.Add(new MonthlyAnalyticsDto
                        {
                            MonthLabel = weekLabel,
                            Month = weekNumber,
                            Years = date.Year,
                            Earnings = 0,
                            Students = 0
                        });
                }
            }

            // Ensure chronological ordering
            return result.OrderBy(x => x.Years).ThenBy(x => x.Month).ToList();
        }
        #endregion

        #region Group By Month Helper
        /// <summary>
        /// Groups enrollments by month and normalizes: fills missing months with zero values.
        /// </summary>
        private List<MonthlyAnalyticsDto> GroupByMonth(IReadOnlyList<Enrollment> enrollments, DateTime fromDate, DateTime toDate)
        {
            // 1. Aggregate actual enrollment data grouped by month
            var grouped = enrollments.GroupBy(x => new { x.CreatedAt.Month, x.CreatedAt.Year })
                .Select(group => new MonthlyAnalyticsDto
                {
                    Month = group.Key.Month,
                    Years = group.Key.Year,
                    MonthLabel = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("yyyy-MM"),
                    Earnings = group.Where(x => x.IsPaid).Sum(x => x.Amount),
                    Students = group.Select(x => x.StudentId).Distinct().Count()
                }).ToList();

            // 2. Normalize: generate all months in range, fill missing with zeros
            var dict = grouped.ToDictionary(x => x.MonthLabel);
            var result = new List<MonthlyAnalyticsDto>();
            var currentMonth = new DateTime(fromDate.Year, fromDate.Month, 1);
            var lastMonth = new DateTime(toDate.Year, toDate.Month, 1);
            while (currentMonth <= lastMonth)
            {
                var label = currentMonth.ToString("yyyy-MM");
                if (dict.TryGetValue(label, out var existing))
                    result.Add(existing);
                else
                    result.Add(new MonthlyAnalyticsDto
                    {
                        MonthLabel = label,
                        Month = currentMonth.Month,
                        Years = currentMonth.Year,
                        Earnings = 0,
                        Students = 0
                    });
                currentMonth = currentMonth.AddMonths(1);
            }
            return result;
        }
        #endregion

        #region Build Enrollment Analytics Spec
        /// <summary>
        /// Generate EnrollmentSpec For Chart Analyze
        /// </summary>
        private BaseSpecifications<Enrollment> BuildEnrollmentAnalyticsSpec(
            int? instructorId,
            ChartRequest req)
        {
            var toDate = req.ToDate ?? DateTime.UtcNow;
            var fromDate = req.FromDate ?? DateTime.UtcNow.AddMonths(-12);
            var spec = new BaseSpecifications<Enrollment>(x =>
                (x.Course.InstructorId == instructorId) &&
                (x.CreatedAt >= fromDate) &&
                (x.CreatedAt <= toDate) &&
                (x.Status == EnrollStatus.Active)
            );

            spec.Includes.Add(x => x.Student);

            return spec;
        }
        #endregion
        private async Task<int?> GetCurrentInstructor()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            if (instructorInfo is null || instructorInfo.Data is null)
                return null;

            return instructorInfo.Data.Id;
        }
    }
}
