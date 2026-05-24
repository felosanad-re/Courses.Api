using AutoMapper;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Services.Contract.StudentServices;
using Courses.Core.Specifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.StudentServices
{
    public class StudentService : IStudentService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentStudentService _currentStudentService;
        protected readonly IMapper _mapper;
        protected readonly ILogger<StudentService> _logger;

        public StudentService(IUnitOfWork unitOfWork, ICurrentStudentService currentStudentService, IMapper mapper, ILogger<StudentService> logger)
        {
            _unitOfWork = unitOfWork;
            _currentStudentService = currentStudentService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApplicationServiceResult<IReadOnlyList<EnrollmentWithCoursesResponse>>> GetAllStudentCoursesAsync()
        {
            var userNotFoundError = "Student Not Found With this id";
            var succeededMessage = "Revived Student Courses Enrollmented Succeeded";
            var loggerError = "there is a problem in database";

            var student = await _currentStudentService.GetStudentWithApplicationUser();
            if (student == null || !student.Succeed || student.Data is null)
                return ApplicationServiceResult<IReadOnlyList<EnrollmentWithCoursesResponse>>.Fail(userNotFoundError);

            try
            {
                var spec = new BaseSpecifications<Enrollment>(x =>
                    x.StudentId == student.Data.Id &&
                    x.Status == EnrollStatus.Active);

                spec.IsTracking = false;
                spec.Includes.Add(x => x.Course);
                spec.IncludesString.Add("Course.CourseType");

                var enrollments = await _unitOfWork.CreateRepository<Enrollment>().GetAllAsyncSpec(spec);

                // if no active courses for this student yet
                if (enrollments.Count == 0) return ApplicationServiceResult<IReadOnlyList<EnrollmentWithCoursesResponse>>
                        .Success(new List<EnrollmentWithCoursesResponse>(), "No active courses found");

                var data = _mapper.Map<IReadOnlyList<EnrollmentWithCoursesResponse>>(enrollments);

                return ApplicationServiceResult<IReadOnlyList<EnrollmentWithCoursesResponse>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve active courses for studentId: {studentId}", student.Data.Id);
                return ApplicationServiceResult<IReadOnlyList<EnrollmentWithCoursesResponse>>.Fail(loggerError);
            }
        }
    }
}
