using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Services.Contract.EnrollmentServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.UnitOfWork;
using Courses.Services.UserServices;
using Microsoft.Extensions.Logging;

namespace Courses.Services.EnrollmentServices
{
    public class EnrollmentService : IEnrollmentService
    {
        #region DI Service
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IMapper _mapper;
        protected readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EnrollmentService> logger, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }
        #endregion

        public async Task<ApplicationServiceResult<EnrollmentWithCourseResponse>> CreateEnrollmentAsync(EnrollmentRequest req)
        {
            var userNotFoundError = "User Not Found With this id";
            var errorMessage = "there is no course with this id";
            var succeededMessage = "Course Enrollment Succeeded";

            var userId = _currentUserService.UserId;
            if (userId == null) return ApplicationServiceResult<EnrollmentWithCourseResponse>.Fail(userNotFoundError);

            var courseRepo = _unitOfWork.CreateRepository<Course>();
            var course = await courseRepo.GetAsyncSpec(new CoursesWithSpec(req.CourseId));
            if (course is null) return ApplicationServiceResult<EnrollmentWithCourseResponse>.Fail(errorMessage);

            // if course is exist
            var existCourse = course.IsPaid; // False
            if(!existCourse) // Free Course
            {
                var enrollment = new Enrollment()
                {
                    CourseId = req.CourseId,
                    StudentId = 1,
                };
                await _unitOfWork.CreateRepository<Enrollment>().AddAsync(enrollment);
            }
            var data = new EnrollmentWithCourseResponse()
            {
                CourseId = course.Id,
                Status = EnrollStatus.Enroll
            };
            return ApplicationServiceResult<EnrollmentWithCourseResponse>.Success(data, succeededMessage);
        }
    }
}
