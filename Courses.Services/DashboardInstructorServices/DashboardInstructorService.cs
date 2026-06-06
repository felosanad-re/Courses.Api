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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            int? instructorId = null;
            // Get Instructor Id
            try
            {
                instructorId = await GetCurrentInstrurInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<DashboardInstructorDTO>.Fail("there is no instructor with this id");

                // Get All Courses
                var courseRepo = _unitOfWork.CreateRepository<Course>();
                var allCourseSpec = new AllCoursesWithInstructorSpec(instructorId);
                var allCourses = await courseRepo.GetCountAsyncSpec(allCourseSpec);

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
                var revenuesSpec = new BaseSpecifications<Enrollment>(x => x.Course.InstructorId == instructorId);

                var allRevenues = await enrollmentRepo.GetSumAsyncSpec(revenuesSpec, x => x.Amount);

                // Get new Revenues
                var newReveunesSpec = new BaseSpecifications<Enrollment>(x =>
                    (x.Course.InstructorId == instructorId) &&
                    (x.CreatedAt >= oneMonthAgo)
                );

                var newReveunes = await enrollmentRepo.GetSumAsyncSpec(newReveunesSpec, x => x.Amount);

                var data = new DashboardInstructorDTO()
                {
                    TotalCourses = allCourses,
                    TotalNewCoursesInMonth = newCourses,
                    TotalStudents = allStudents,
                    NewTotalStudentsInMonth = newStudents,
                    TotalRevenues = allRevenues,
                    NewTotalRevenuesInMonth = newReveunes
                };
                return ApplicationServiceResult<DashboardInstructorDTO>.Success(data, "you retrieve all dashboard stats succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is error when try to retrieve status for instructor id  {instructorId}", instructorId);
                return ApplicationServiceResult<DashboardInstructorDTO>.Fail("There Is problem In database");
            }
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
