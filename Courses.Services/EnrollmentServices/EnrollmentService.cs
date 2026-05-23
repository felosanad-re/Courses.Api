using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Services.Contract.EnrollmentServices;
using Courses.Core.Services.Contract.StudentServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.EnrollmentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.EnrollmentServices
{
    public class EnrollmentService : IEnrollmentService
    {
        #region DI Service
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ICurrentStudentService _currentStudentService;
        protected readonly IMapper _mapper;
        protected readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EnrollmentService> logger, ICurrentUserService currentUserService, ICurrentStudentService currentStudentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
            _currentStudentService = currentStudentService;
        }
        #endregion

        #region Create Enrollment Async
        public async Task<ApplicationServiceResult<EnrollmentWithCourseResponse>> CreateEnrollmentAsync(EnrollmentRequest req)
        {
            var userNotFoundError = "Student Not Found With this id";
            var errorMessage = "there is no course with this id";
            var succeededMessageFree = "Course Enrollment Succeeded";
            var succeededMessagePaid = "Payment required to complete enrollment";
            var loggerError = "there is a problem in database";
            var enrollStudentMessage = "this student already enroll in this course";
            // Get Current USer
            var userId = _currentUserService.UserId;

            // get Current Student Id
            var student = await _currentStudentService.GetStudentWithApplicationUser();
            if (student == null) return ApplicationServiceResult<EnrollmentWithCourseResponse>.Fail(userNotFoundError);
            var studentId = student.Data!.Id;

            try
            {
                var courseRepo = _unitOfWork.CreateRepository<Course>();
                var course = await courseRepo.GetAsyncSpec(new CoursesWithSpec(req.CourseId));
                if (course is null) return ApplicationServiceResult<EnrollmentWithCourseResponse>
                        .Fail(errorMessage);

                // check if student enrollment in this course
                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var isEnrollment = await enrollmentRepo
                    .GetAsyncSpec(new EnrollmentWithSpec(studentId, course.Id));

                if (isEnrollment is not null)
                    return ApplicationServiceResult<EnrollmentWithCourseResponse>
                    .Fail(enrollStudentMessage);

                // if course is exist
                var isPaidCourse = course.IsPaid; // False

                // Free Course
                if (!isPaidCourse)
                {
                    var freeEnrollment = new Enrollment()
                    {
                        CourseId = req.CourseId,
                        StudentId = studentId,
                        Progress = 0m,
                        IsCompleted = false,
                        CreatedBy = student.Data.Name,
                        Status = EnrollStatus.Active,
                        IsPaid = false,
                        Amount = 0m
                    };
                    await enrollmentRepo.AddAsync(freeEnrollment);
                    await _unitOfWork.CompleteAsync();

                    var data = new EnrollmentWithCourseResponse()
                    {
                        EnrollmentId = freeEnrollment.Id,
                        CourseId = course.Id,
                        Status = EnrollStatus.Active,
                        UserId = userId ?? string.Empty,
                        IsPaid = false,
                        Amount = 0m
                    };
                    return ApplicationServiceResult<EnrollmentWithCourseResponse>.Success(data, succeededMessageFree);
                }

                // Paid Course
                var paidEnrollment = new Enrollment()
                {
                    CourseId = req.CourseId,
                    StudentId = studentId,
                    Progress = 0m,
                    IsCompleted = false,
                    CreatedBy = student.Data.Name,
                    Status = EnrollStatus.PendingPayment,
                    IsPaid = true,
                    Amount = course.Price
                };

                await enrollmentRepo.AddAsync(paidEnrollment);
                await _unitOfWork.CompleteAsync();

                var paidEnrollmentResponse = new EnrollmentWithCourseResponse()
                {
                    EnrollmentId = paidEnrollment.Id,
                    CourseId = course.Id,
                    Status = paidEnrollment.Status,
                    UserId = userId ?? string.Empty,
                    IsPaid = true,
                    Amount = course.Price
                };

                return ApplicationServiceResult<EnrollmentWithCourseResponse>.Success(paidEnrollmentResponse, succeededMessagePaid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Field To create enrollment for courseId: {courseId}", req.CourseId);
                return ApplicationServiceResult<EnrollmentWithCourseResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Update Enrollment Status
        public async Task<ApplicationServiceResult<UpdateEnrollmentResponse>> UpdateEnrollmentStatusAsync(string paymentIntentId, EnrollStatus status)
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId))
                return ApplicationServiceResult<UpdateEnrollmentResponse>.Fail("Payment intent id is required");

            try
            {
                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var enrollment = await enrollmentRepo.GetAsyncSpec(new EnrollmentWithSpec(paymentIntentId));

                if (enrollment is null)
                    return ApplicationServiceResult<UpdateEnrollmentResponse>.Fail("No enrollment found with this payment intent id");

                enrollment.Status = status;
                await _unitOfWork.CompleteAsync();

                var response = new UpdateEnrollmentResponse
                {
                    EnrollmentId = enrollment.Id,
                    Status = enrollment.Status,
                    PaymentIntentId = enrollment.PaymentIntentId
                };

                return ApplicationServiceResult<UpdateEnrollmentResponse>.Success(response, "Enrollment status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update enrollment status for paymentIntentId: {paymentIntentId}", paymentIntentId);
                return ApplicationServiceResult<UpdateEnrollmentResponse>.Fail("There is a problem in database");
            }
        }
        #endregion
    }
}
