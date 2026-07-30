using AutoMapper;
using Courses.Core.Models;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.ModelsDTO.ResponseDTO.Charts;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.EnrollmentSpecifications;
using Courses.Core.Specifications.InstructorsSpecifications;
using Courses.Core.Specifications.RatingSpecifications;
using Courses.Core.Specifications.StudentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminDashboardService : IAdminDashboardService
    {
        #region Service
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IMapper _mapper;
        protected readonly ILogger<AdminDashboardService> _logger;

        public AdminDashboardService(IUnitOfWork unitOfWork, ILogger<AdminDashboardService> logger, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        #endregion

        #region Get Stats Async
        public async Task<ApplicationServiceResult<AdminDashboardStatsResponse>> GetStatsAsync()
        {
            const string SucceededMessage = "Dashboard statistics retrieved successfully.";
            const string LoggerMessage = "Failed to retrieve dashboard statistics.";
            string? userId = _currentUserService.UserId;

            // Initialize REPOS
            try
            {
                var courseRepo = _unitOfWork.CreateRepository<Course>();
                var studentRepo = _unitOfWork.CreateRepository<Student>();
                var instructorRepo = _unitOfWork.CreateRepository<Instructor>();
                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();

                // Get Courses Count
                var courseSpec = new CoursesCountWithSpec();
                var coursesCount = await courseRepo.GetCountAsyncSpec(courseSpec);

                // Get Student Count
                var studentSpec = new BaseSpecifications<Student>();
                var studentCount = await studentRepo.GetCountAsyncSpec(studentSpec);
                // Get Instructors Count
                var instructorSpec = new BaseSpecifications<Instructor>();
                var instructorCount = await instructorRepo.GetCountAsyncSpec(instructorSpec);
                // Get Users Count
                var usersCount = studentCount + instructorCount;
                // Get Published Courses Count
                var publishedCoursesSpec = new CoursesCountWithSpec(CourseStatus.Published);
                var publishedCoursesCount = await courseRepo.GetCountAsyncSpec(publishedCoursesSpec);
                // Get Total Revenue
                var enrollmentSpec = new EnrollmentWithSpec();
                var revenueCount = await enrollmentRepo.GetSumAsyncSpec(enrollmentSpec, x => x.Amount);
                var data = new AdminDashboardStatsResponse
                {
                    Courses = coursesCount,
                    Instructors = instructorCount,
                    PublishedCourses = publishedCoursesCount,
                    Revenue = revenueCount,
                    Students = studentCount,
                    Users = usersCount,
                };

                return ApplicationServiceResult<AdminDashboardStatsResponse>.Success(data, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve admin dashboard stats userId {userId}", userId);
                return ApplicationServiceResult<AdminDashboardStatsResponse>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region GetChartsAsync
        public async Task<ApplicationServiceResult<AdminDashboardChartsResponse>> GetChartsAsync(DateTime? fromDate, DateTime? toDate)
        {
            const string SucceededMessage = "Dashboard charts retrieved successfully.";
            const string LoggerMessage = "Failed to retrieve dashboard charts.";
            string? userId = _currentUserService.UserId;

            try
            {
                var start = fromDate ?? DateTime.UtcNow.AddMonths(-12); // show the last Year
                var end = toDate ?? DateTime.UtcNow;
                var range = (end - start).TotalDays; // last month| Last 6 months | last year

                var studentSpec = new StudentSpec(start, end);
                var enrollmentSpec = new EnrollmentWithSpec(start, end);

                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();

                // Students
                var studentQuery = _unitOfWork.CreateRepository<Student>().GetQuerySpec(studentSpec);

                // Enrollments
                var enrollmentQuery = enrollmentRepo.GetQuerySpec(enrollmentSpec);

                List<ChartPointResponse> studentChart;
                List<ChartPointResponse> enrollmentChart;
                List<ChartPointResponse> revenueChart;

                if (range <= 30)
                {
                    var studentGrouped = await studentQuery.GroupBy(x => new
                    {
                        x.CreatedAt.Date
                    })
                    .Select(x => new ChartPointResponse
                    {
                        Label = x.Key.Date.ToString("yyyy-MM-dd"),
                        Value = x.Count()
                    }).ToDictionaryAsync(x => x.Label);

                    var enrollmentGrouped = await enrollmentQuery
                    .GroupBy(x => new
                    {
                        x.CreatedAt.Date
                    })
                    .Select(x => new ChartPointResponse
                     {
                         Label = x.Key.Date.ToString("yyyy-MM-dd"),
                         Value = x.Count()
                     }).ToDictionaryAsync(x => x.Label);

                    // Revenue
                    var revenueGrouped = await enrollmentQuery
                        .GroupBy(x => new
                        {
                            x.CreatedAt.Date
                        })
                        .Select(x => new ChartPointResponse
                        {
                            Label= x.Key.Date.ToString("yyyy-MM-dd"),
                            Value = x.Sum(x => x.Amount)
                        }).ToDictionaryAsync(x => x.Label);

                    studentChart = NormalizeByDay(studentGrouped, start, end);
                    enrollmentChart = NormalizeByDay(enrollmentGrouped, start, end);
                    revenueChart = NormalizeByDay(revenueGrouped, start, end);
                }

                else
                {
                    var studentGrouped = await studentQuery
                        .GroupBy(x => new
                        {
                            x.CreatedAt.Year,
                            x.CreatedAt.Month
                        })
                        .Select(x => new ChartPointResponse
                        {
                            Label = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("yyyy-MM"),
                            Value = x.Count()
                        }).ToDictionaryAsync(x => x.Label);

                    var enrollmentGrouped = await enrollmentQuery
                        .GroupBy(x => new
                        {
                            x.CreatedAt.Year,
                            x.CreatedAt.Month
                        })
                        .Select(x => new ChartPointResponse
                        {
                            Label = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("yyyy-MM"),
                            Value = x.Count()
                        }).ToDictionaryAsync(x => x.Label);

                    // Revenue
                    var revenueGrouped = await enrollmentQuery
                        .GroupBy(x => new
                        {
                            x.CreatedAt.Year,
                            x.CreatedAt.Month
                        })
                        .Select(x => new ChartPointResponse
                        {
                            Label = new DateTime(x.Key.Year, x.Key.Month, 1)
                            .ToString("yyyy-MM"),
                            Value = x.Sum(x => x.Amount)
                        }).ToDictionaryAsync(x => x.Label);

                    studentChart = NormalizeByMonth(studentGrouped, start, end);
                    enrollmentChart = NormalizeByMonth(enrollmentGrouped, start, end);
                    revenueChart = NormalizeByMonth(revenueGrouped, start, end);
                }

                var charts = new AdminDashboardChartsResponse
                {
                    Charts = new List<DashboardChartsResponse>()
                {
                    // Student
                    new DashboardChartsResponse
                    {
                        Title = "Student",
                        Key = "student",
                        Data = studentChart
                    },
                    // Enrollment
                    new DashboardChartsResponse
                    {
                        Title = "Enrollment",
                        Key = "enrollment",
                        Data = enrollmentChart
                    },
                    // Revenue
                    new DashboardChartsResponse
                    {
                        Title = "Revenue",
                        Key = "revenue",
                        Data = revenueChart
                    }
                }
                };

                return ApplicationServiceResult<AdminDashboardChartsResponse>.Success(charts, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem retrieving admin dashboard charts for userId {UserId}", userId);
                return ApplicationServiceResult<AdminDashboardChartsResponse>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region Get Latest Reviews Async
        public async Task<ApplicationServiceResult<List<AdminDashboardReviewsResponse>>> GetLatestReviewsAsync()
        {
            const string SucceededMessage = "Dashboard charts retrieved successfully.";
            const string WarnningMessage = "No reviews found.";
            const string LoggerMessage = "Failed to retrieve dashboard charts.";
            string? userId = _currentUserService.UserId;

            try
            {
                var courseRatingRepo = _unitOfWork.CreateRepository<CourseRating>();
                var courseRatingSpec = new RatingWithSpec();

                var ratings = await courseRatingRepo.GetAllAsyncSpec(courseRatingSpec);
                if (!ratings.Any())
                    return ApplicationServiceResult<List<AdminDashboardReviewsResponse>>.Success(new(), WarnningMessage);

                var ratingData = _mapper.Map<List<AdminDashboardReviewsResponse>>(ratings);
                return ApplicationServiceResult<List<AdminDashboardReviewsResponse>>.Success(ratingData, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Reviews for Admin {userId}", userId);
                return ApplicationServiceResult<List<AdminDashboardReviewsResponse>>.Fail(LoggerMessage);

            }
        }
        #endregion

        #region Get Quick Actions Async
        public async Task<ApplicationServiceResult<AdminDashboardQuickActionsResponse>> GetQuickActionsAsync()
        {
            const string SucceededMessage = "Dashboard quick actions retrieved successfully.";
            const string LoggerMessage = "Failed to retrieve dashboard Quick Actions.";
            string? userId = _currentUserService.UserId;

            try
            {
                var draftCoursesSpec = new CoursesCountWithSpec(CourseStatus.Draft);
                var pendingCoursesSpec = new CoursesCountWithSpec(CourseStatus.PendingReview);
                var pendingInstructorsSpec = new InstructorSpec(InstructorStatus.Pending);

                var coursesRepo = _unitOfWork.CreateRepository<Course>();
                var instructorRepo = _unitOfWork.CreateRepository<Instructor>();
                // Draft Courses
                var draftCoursesCount = await coursesRepo.GetCountAsyncSpec(draftCoursesSpec);

                // Pending Courses
                var pendingCoursesCount = await coursesRepo.GetCountAsyncSpec(pendingCoursesSpec);

                // Pending Instructors
                var pendingInstructorsCount = await instructorRepo.GetCountAsyncSpec(pendingInstructorsSpec);

                var data = new AdminDashboardQuickActionsResponse
                {
                    DraftCoursesCount = draftCoursesCount,
                    PendingInstructorsCount = pendingInstructorsCount,
                    PendingCoursesCount = pendingCoursesCount,
                };

                return ApplicationServiceResult<AdminDashboardQuickActionsResponse>.Success(data, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Quick Actions for Admin {userId}", userId);
                return ApplicationServiceResult<AdminDashboardQuickActionsResponse>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region Helper Method
        private List<ChartPointResponse> NormalizeByDay(Dictionary<string, ChartPointResponse> dict, DateTime fromDate, DateTime toDate)
        {
            // Normalize
            var result = new List<ChartPointResponse>();
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(+1))
            {
                var label = date.ToString("yyyy-MM-dd");

                if (dict.TryGetValue(label, out var item))
                    result.Add(item);
                else
                    result.Add(new ChartPointResponse
                    {
                        Label = label,
                        Value = 0
                    });
            }

            return result;
        }

        private List<ChartPointResponse> NormalizeByMonth(Dictionary<string, ChartPointResponse> dict, DateTime fromDate, DateTime toDate)
        {
            // Normalize
            var result = new List<ChartPointResponse>();
            var currentMonth = new DateTime(fromDate.Year, fromDate.Month, 1);
            var lastMonth = new DateTime(toDate.Year, toDate.Month, 1);
            while (currentMonth <= lastMonth)
            {
                var label = currentMonth.ToString("yyyy-MM");
                if (dict.TryGetValue(label, out var item))
                    result.Add(item);
                else
                    result.Add(new ChartPointResponse
                    {
                        Label = label,
                        Value = 0
                    });
                currentMonth = currentMonth.AddMonths(1);
            }

            return result;
        }
        #endregion
    }
}
