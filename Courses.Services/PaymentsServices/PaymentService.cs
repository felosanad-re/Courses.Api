using AutoMapper;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Payments;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.ModelsDTO.ResponseDTO.Payments;
using Courses.Core.Options;
using Courses.Core.Services.Contract.PaymentsServices;
using Courses.Core.Services.Contract.StudentServices;
using Courses.Core.Specifications.EnrollmentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace Courses.Services.PaymentsServices
{
    public class PaymentService : IPaymentService
    {
        #region DI Service
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentStudentService _currentStudentService;
        protected readonly ILogger<PaymentService> _logger;
        protected readonly IMapper _mapper;
        protected readonly StripeOptions _stripeOptions;

        public PaymentService(IUnitOfWork unitOfWork, ILogger<PaymentService> logger, IMapper mapper, ICurrentStudentService currentStudentService, IOptions<StripeOptions> stripeOptions)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _currentStudentService = currentStudentService;
            _stripeOptions = stripeOptions.Value;
        }
        #endregion

        #region Create Payment Intent Async
        public async Task<ApplicationServiceResult<PaymentResponse>> CreatePaymentIntent(PaymentRequest req)
        {
            var studentId = await ValidateCurrentStudentAsync();
            if (studentId is null)
                return ApplicationServiceResult<PaymentResponse>.Fail("Student not found with this user id");

            try
            {
                // Validate enrollment belongs to the current student
                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var spec = new EnrollmentWithSpec(req.EnrollmentId);
                var enrollment = await enrollmentRepo.GetAsyncSpec(spec);

                if (enrollment is null)
                    return ApplicationServiceResult<PaymentResponse>.Fail("No enrollment found with this id");

                // Verify enrollment belongs to the current student
                if (enrollment.StudentId != studentId)
                    return ApplicationServiceResult<PaymentResponse>.Fail("This enrollment does not belong to the current student");

                // If enrollment already has a successful payment, skip creating new intent
                if (enrollment.PaymentIntentId is not null && enrollment.Status == EnrollStatus.PaymentSucceeded)
                    return ApplicationServiceResult<PaymentResponse>.Fail("Payment already completed for this enrollment");

                var course = enrollment.Course;

                if (course is null)
                    return ApplicationServiceResult<PaymentResponse>.Fail("Course not found for this enrollment");

                if (course.Price <= 0)
                    return ApplicationServiceResult<PaymentResponse>.Fail("Course price must be greater than zero");

                var service = new PaymentIntentService();

                if (!string.IsNullOrWhiteSpace(enrollment.PaymentIntentId))
                {
                    var existingPaymentIntent = await service.GetAsync(enrollment.PaymentIntentId);
                    enrollment.Status = MapStripeStatusToEnrollmentStatus(existingPaymentIntent.Status);
                    await _unitOfWork.CompleteAsync();

                    return ApplicationServiceResult<PaymentResponse>.Success(
                        BuildPaymentResponse(enrollment, existingPaymentIntent),
                        "Payment intent retrieved successfully");
                }

                // Create Stripe PaymentIntent
                var options = BuildStripePaymentIntentOptions(course.Price, course.Name, course.Id, req.EnrollmentId);
                var paymentIntent = await service.CreateAsync(options);

                // Update enrollment with PaymentIntentId and status
                enrollment.PaymentIntentId = paymentIntent.Id;
                enrollment.Status = EnrollStatus.PendingPayment;

                await _unitOfWork.CompleteAsync();

                return ApplicationServiceResult<PaymentResponse>.Success(
                    BuildPaymentResponse(enrollment, paymentIntent),
                    "Payment intent created successfully");
            }
            catch (StripeException stripeEx)
            {
                _logger.LogError(stripeEx, "Stripe error while creating payment intent for enrollment: {enrollmentId}", req.EnrollmentId);
                return ApplicationServiceResult<PaymentResponse>.Fail($"Stripe payment error: {stripeEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create payment intent for enrollment: {enrollmentId}", req.EnrollmentId);
                return ApplicationServiceResult<PaymentResponse>.Fail("There is a problem in database");
            }
        }
        #endregion

        #region Get Payment Intent Async
        public async Task<ApplicationServiceResult<PaymentResponse>> GetPaymentIntent(string paymentIntentId)
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId))
                return ApplicationServiceResult<PaymentResponse>.Fail("Payment intent id is required");

            var studentId = await ValidateCurrentStudentAsync();
            if (studentId is null)
                return ApplicationServiceResult<PaymentResponse>.Fail("Student not found with this user id");

            try
            {
                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var enrollment = await enrollmentRepo.GetAsyncSpec(new EnrollmentWithSpec(paymentIntentId));

                if (enrollment is null)
                    return ApplicationServiceResult<PaymentResponse>.Fail("No enrollment found with this payment intent id");

                if (enrollment.StudentId != studentId)
                    return ApplicationServiceResult<PaymentResponse>.Fail("This payment intent does not belong to the current student");

                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                enrollment.Status = MapStripeStatusToEnrollmentStatus(paymentIntent.Status);
                await _unitOfWork.CompleteAsync();

                return ApplicationServiceResult<PaymentResponse>.Success(
                    BuildPaymentResponse(enrollment, paymentIntent),
                    "Payment intent retrieved successfully");
            }
            catch (StripeException stripeEx)
            {
                _logger.LogError(stripeEx, "Stripe error while retrieving payment intent: {paymentIntentId}", paymentIntentId);
                return ApplicationServiceResult<PaymentResponse>.Fail($"Stripe payment error: {stripeEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve payment intent: {paymentIntentId}", paymentIntentId);
                return ApplicationServiceResult<PaymentResponse>.Fail("There is a problem in database");
            }
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Validates the current student and returns their ID.
        /// Returns null if the student is not found or invalid.
        /// </summary>
        private async Task<int?> ValidateCurrentStudentAsync()
        {
            var student = await _currentStudentService.GetStudentWithApplicationUser();
            if (student is null || !student.Succeed || student.Data is null)
                return null;

            return student.Data.Id;
        }

        private static long ConvertPriceToCents(decimal price)
            => (long)Math.Round(price * 100, MidpointRounding.AwayFromZero);

        private static EnrollStatus MapStripeStatusToEnrollmentStatus(string? stripeStatus)
        {
            return stripeStatus switch
            {
                "succeeded" => EnrollStatus.PaymentSucceeded,
                "canceled" => EnrollStatus.PaymentCancelled,
                _ => EnrollStatus.PendingPayment
            };
        }

        private static PaymentResponse BuildPaymentResponse(Enrollment enrollment, PaymentIntent paymentIntent)
        {
            return new PaymentResponse
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                EnrollmentId = enrollment.Id,
                CourseId = enrollment.CourseId,
                Status = enrollment.Status.ToString()
            };
        }

        /// <summary>
        /// Creates a Stripe PaymentIntent for the given course price.
        /// </summary>
        private PaymentIntentCreateOptions BuildStripePaymentIntentOptions(decimal coursePrice, string courseName, int courseId, int enrollmentId)
        {
            return new PaymentIntentCreateOptions
            {
                Amount = ConvertPriceToCents(coursePrice),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    { "courseId", courseId.ToString() },
                    { "courseName", courseName },
                    { "enrollmentId", enrollmentId.ToString() }
                }
            };
        }

        #endregion
    }
}
