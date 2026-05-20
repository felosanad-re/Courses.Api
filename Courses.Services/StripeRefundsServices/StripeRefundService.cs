using AutoMapper;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Refunds;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.ModelsDTO.ResponseDTO.Refunds;
using Courses.Core.Services.Contract.ProgressServices;
using Courses.Core.Services.Contract.RefundsServices;
using Courses.Core.Services.Contract.StudentServices;
using Courses.Core.Specifications.EnrollmentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Courses.Services.StripeRefundsServices
{
    public class StripeRefundService : IRefundService
    {
        #region DI Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentStudentService _currentStudentService;
        protected readonly IProgressService _progressService;
        protected readonly IMapper _mapper;
        protected readonly ILogger<StripeRefundService> _logger;

        public StripeRefundService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<StripeRefundService> logger, IProgressService progressService, ICurrentStudentService currentStudentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _progressService = progressService;
            _currentStudentService = currentStudentService;
        }
        #endregion

        #region Create Refund Request Async
        public async Task<ApplicationServiceResult<RefundResponse>> CreateRefundRequestAsync(RefundRequest req)
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (studentId is null)
                return ApplicationServiceResult<RefundResponse>.Fail("Student not found with this user id");

            try
            {
                var enrollment = await GetEnrollmentWithCourseAsync(req.EnrollmentId);

                // Check on course before Make Refund
                var validationError = await ValidateRefundRequestAsync(enrollment, studentId.Value);

                if (validationError is not null) // there is error
                    return ApplicationServiceResult<RefundResponse>.Fail(validationError);

                var refundService = new Stripe.RefundService();
                var refund = await refundService.CreateAsync(BuildStripeRefundOptions(enrollment!));

                enrollment!.CancellationReason = req.CancellationReason;
                enrollment.Status = EnrollStatus.RefundedPending;

                await _unitOfWork.CompleteAsync();

                return ApplicationServiceResult<RefundResponse>.Success(
                    BuildRefundResponse(enrollment, refund),
                    "Refund request sent successfully");
            }
            catch (StripeException stripeEx)
            {
                _logger.LogError(stripeEx, "Stripe error while creating refund for enrollment: {enrollmentId}", req.EnrollmentId);
                return ApplicationServiceResult<RefundResponse>.Fail($"Stripe refund error: {stripeEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create refund for enrollment: {enrollmentId}", req.EnrollmentId);
                return ApplicationServiceResult<RefundResponse>.Fail("There is a problem in database");
            }
        }
        #endregion

        #region Update Refund Status Async
        public async Task<ApplicationServiceResult<RefundResponse>> UpdateRefundStatusAsync(string paymentIntentId, EnrollStatus status)
        {
            try
            {
                var enrollment = await GetEnrollmentWithCourseFouUpdateStatusAsync(paymentIntentId);
                if (enrollment is null)
                    return ApplicationServiceResult<RefundResponse>.Fail("No enrollment found with this id");

                if (string.IsNullOrWhiteSpace(enrollment.PaymentIntentId))
                    return ApplicationServiceResult<RefundResponse>.Fail("Payment intent id is required");

                enrollment.Status = status;
                await _unitOfWork.CompleteAsync();

                return ApplicationServiceResult<RefundResponse>.Success(
                    BuildRefundResponse(enrollment),
                    "Refund status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update refund status for enrollment: {enrollmentId}", paymentIntentId);
                return ApplicationServiceResult<RefundResponse>.Fail("There is a problem in database");
            }
        }
        #endregion

        #region Helper Methods

        private async Task<int?> GetCurrentStudentIdAsync()
        {
            var student = await _currentStudentService.GetStudentWithApplicationUser();
            if (student is null || !student.Succeed || student.Data is null)
                return null;

            return student.Data.Id;
        }

        // For Update Status
        private async Task<Enrollment?> GetEnrollmentWithCourseFouUpdateStatusAsync(string paymentIntentId)
        {
            var spec = new EnrollmentWithSpec(paymentIntentId);
            return await _unitOfWork.CreateRepository<Enrollment>().GetAsyncSpec(spec);
        }

        // For Create Refunded Request
        private async Task<Enrollment?> GetEnrollmentWithCourseAsync(int enrollmentId)
        {
            var spec = new EnrollmentWithSpec(enrollmentId);
            return await _unitOfWork.CreateRepository<Enrollment>().GetAsyncSpec(spec);
        }

        private async Task<string?> ValidateRefundRequestAsync(Enrollment? enrollment, int studentId)
        {
            if (enrollment is null)
                return "No enrollment found with this id";

            if (enrollment.StudentId != studentId)
                return "This enrollment does not belong to the current student";

            if (enrollment.Status != EnrollStatus.Active)
                return "Only active paid enrollments can be refunded";

            if (!enrollment.IsPaid)
                return "Free courses cannot be refunded";

            if (string.IsNullOrWhiteSpace(enrollment.PaymentIntentId))
                return "Payment intent not found";

            if (enrollment.Course is null)
                return "Course not found for this enrollment";

            if (enrollment.Course.Price <= 0)
                return "Course price must be greater than zero";

            if (DateTime.UtcNow > enrollment.CreatedAt.AddDays(15))
                return "You can't refund this course after 15 days";

            var courseProgress = await _progressService.GetCourseProgressAsync(enrollment.CourseId);
            if (!courseProgress.Succeed || courseProgress.Data is null)
                return courseProgress.Message ?? "Could not retrieve course progress";

            if (courseProgress.Data.ProgressPercentage >= 30)
                return "You can't refund a course after completing 30% or more";

            return null;
        }

        private static RefundCreateOptions BuildStripeRefundOptions(Enrollment enrollment)
        {
            return new RefundCreateOptions
            {
                PaymentIntent = enrollment.PaymentIntentId,
                Metadata = new Dictionary<string, string>
                {
                    { "courseId", enrollment.CourseId.ToString() },
                    { "enrollmentId", enrollment.Id.ToString() },
                    { "studentId", enrollment.StudentId.ToString() },
                    { "paymentIntentId", enrollment.PaymentIntentId ?? string.Empty }
                }
            };
        }

        private static RefundResponse BuildRefundResponse(Enrollment enrollment, Refund? refund = null)
        {
            return new RefundResponse
            {
                CancellationReason = enrollment.CancellationReason,
                CourseId = enrollment.CourseId,
                PaymentIntentId = enrollment.PaymentIntentId ?? string.Empty,
                Status = enrollment.Status.ToString(),
                EnrollmentId = enrollment.Id
            };
        }

        #endregion
    }
}
