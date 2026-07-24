using AutoMapper;
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
        public async Task<ApplicationServiceResult<AdminDashboardChartsResponse>> GetChartsAsync()
        {
            const string SucceededMessage = "Dashboard charts retrieved successfully.";
            const string LoggerMessage = "Failed to retrieve dashboard charts.";
            string? userId = _currentUserService.UserId;

            try
            {
                var fromDate = DateTime.UtcNow.AddDays(-30); // show the last 30 days

                var studentSpec = new StudentSpec(fromDate);
                var enrollmentSpec = new EnrollmentWithSpec(fromDate);

                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();

                // Students
                var studentQuery = _unitOfWork.CreateRepository<Student>().GetQuerySpec(studentSpec);

                var studentChart = await studentQuery
                .GroupBy(x => new
                {
                    x.CreatedAt.Year,
                    x.CreatedAt.Month
                })
                .OrderBy(x => x.Key.Year)
                .ThenBy(x => x.Key.Month)
                .Select(x => new ChartPointResponse
                {
                    Lable = $"{x.Key.Year}/{x.Key.Month}",
                    Value = x.Count()
                }).ToListAsync();

                // Enrollments
                var enrollmentQuery = enrollmentRepo.GetQuerySpec(enrollmentSpec);

                var enrollmentChart = await enrollmentQuery
                .GroupBy(x => new
                {
                    x.CreatedAt.Year,
                    x.CreatedAt.Month
                })
                .OrderBy(x => x.Key.Year)
                .ThenBy(x => x.Key.Month)
                .Select(x => new ChartPointResponse
                {
                    Lable = $"{x.Key.Year}/{x.Key.Month}",
                    Value = x.Count()
                }).ToListAsync();

                // Revenue
                var revenueChart = await enrollmentQuery
                .GroupBy(x => new
                {
                    x.CreatedAt.Year,
                    x.CreatedAt.Month
                })
                .OrderBy(x => x.Key.Year)
                .ThenBy(x => x.Key.Month)
                .Select(x => new ChartPointResponse
                {
                    Lable = $"{x.Key.Year}/{x.Key.Month}",
                    Value = x.Sum(x => x.Amount)
                }).ToListAsync();

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
                var draftCoursesCountTask = coursesRepo.GetCountAsyncSpec(draftCoursesSpec);

                // Pending Courses
                var pendingCoursesCountTask = coursesRepo.GetCountAsyncSpec(pendingCoursesSpec);

                // Pending Instructors
                var pendingInstructorsCountTask = instructorRepo.GetCountAsyncSpec(pendingInstructorsSpec);

                await Task.WhenAll(pendingCoursesCountTask, pendingCoursesCountTask, pendingCoursesCountTask);

                var data = new AdminDashboardQuickActionsResponse
                {
                    DraftCoursesCount = await draftCoursesCountTask,
                    PendingInstructorsCount = await pendingInstructorsCountTask,
                    PendingCoursesCount = await pendingCoursesCountTask,
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
    }
}
