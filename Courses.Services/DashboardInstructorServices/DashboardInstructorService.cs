using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.DashboardInstructor;
using Courses.Core.Services.Contract.DashboardServices;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace Courses.Services.DashboardInstructorServices
{
    public class DashboardInstructorService : IDashboardInstructorService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<DashboardInstructorService> _logger;
        protected readonly ICurrentInstructorServices _currentInstructorServices;

        public DashboardInstructorService(IUnitOfWork unitOfWork, ILogger<DashboardInstructorService> logger, ICurrentInstructorServices currentInstructorServices)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentInstructorServices = currentInstructorServices;
        }

        public async Task<ApplicationServiceResult<DashboardInstructorDTO>> GetDashboardInstructStatsAsync()
        {

            // Get Instructor Id
            var instructorId = await GetCurrentInstrurInfo();
            if (instructorId is null)
                return ApplicationServiceResult<DashboardInstructorDTO>.Fail("there is no instructor with this id");

            // Get All Courses
            var courseRepo = _unitOfWork.CreateRepository<Course>();
            var spec = new AllCoursesWithInstructorSpec(instructorId);
            var allCourses = await courseRepo.GetCountAsyncSpec(spec);

            // Get New Courses
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            var newCourseSpec = new BaseSpecifications<Course>(x =>
                    (x.InstructorId == instructorId) &&
                    (x.CreatedAt >= oneMonthAgo)
                );
            var newCourses = await courseRepo.GetCountAsyncSpec(newCourseSpec);

            // Get All Students [Enrollment]
            var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
            var enrollmentSpec = new BaseSpecifications<Enrollment>(x => x.Course.InstructorId == instructorId);

            var allStudents = await enrollmentRepo.GetCountAsyncSpec(enrollmentSpec);

            // Get New Students
            var newEnrollmentSpec = new BaseSpecifications<Enrollment>(x =>
                (x.Course.InstructorId == instructorId) &&
                (x.CreatedAt >= oneMonthAgo)
            );
            var newStudents = await enrollmentRepo.GetCountAsyncSpec(newEnrollmentSpec);

            // Get All Revenues [Enrollment]

            // Get new Revenues

            return ApplicationServiceResult<DashboardInstructorDTO>.Fail("");
        }

        #region Helper Methods
        private async Task<int?> GetCurrentInstrurInfo()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            if(instructorInfo is null || !instructorInfo.Succeed || instructorInfo.Data is null)
                return null;

            return instructorInfo.Data?.Id;
        }
        #endregion
    }
}
