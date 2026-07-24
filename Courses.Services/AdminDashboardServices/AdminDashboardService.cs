using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.EnrollmentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminDashboardService : IAdminDashboardService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ILogger<AdminDashboardService> _logger;

        public AdminDashboardService(IUnitOfWork unitOfWork, ILogger<AdminDashboardService> logger, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

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
        public Task<ApplicationServiceResult<AdminDashboardChartsResponse>> GetChartsAsync()
        {
            const string SucceededMessage = "Dashboard statistics retrieved successfully.";
            const string LoggerMessage = "Failed to retrieve dashboard statistics.";
            string? userId = _currentUserService.UserId;

            // Students

            // Enrollments

            // Revenue
            throw new NotImplementedException();
        }
        #endregion
    }
}
