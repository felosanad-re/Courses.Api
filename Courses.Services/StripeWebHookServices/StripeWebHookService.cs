using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Options;
using Courses.Core.Services.Contract.StripeWebHookServices;
using Courses.Core.Specifications.EnrollmentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace Courses.Services.StripeWebHookServices
{
    public class StripeWebHookService : IStripeWebHookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StripeWebHookService> _logger;
        private readonly StripeOptions _stripeOptions;

        public StripeWebHookService(
            IUnitOfWork unitOfWork,
            ILogger<StripeWebHookService> logger,
            IOptions<StripeOptions> stripeOptions)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _stripeOptions = stripeOptions.Value;
        }

        public async Task<bool> WebHook(string json, string stripeSignature)
        {
            if (string.IsNullOrWhiteSpace(_stripeOptions.WebHookSecret))
            {
                _logger.LogError("Stripe webhook secret is not configured.");
                return false;
            }

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _stripeOptions.WebHookSecret);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid Stripe webhook payload or signature.");
                return false;
            }

            if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
            {
                _logger.LogInformation("Stripe webhook event {EventType} ignored because object is not a payment intent.", stripeEvent.Type);
                return true;
            }

            var status = stripeEvent.Type switch
            {
                "payment_intent.succeeded" => EnrollStatus.PaymentSucceeded,
                "payment_intent.payment_failed" => EnrollStatus.PaymentFailed,
                "payment_intent.canceled" => EnrollStatus.PaymentCancelled,
                _ => (EnrollStatus?)null
            };

            if (status is null)
            {
                _logger.LogInformation("Stripe webhook event {EventType} ignored.", stripeEvent.Type);
                return true;
            }

            var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
            var enrollment = await enrollmentRepo.GetAsyncSpec(new EnrollmentWithSpec(paymentIntent.Id));

            if (enrollment is null)
            {
                _logger.LogWarning("No enrollment found for Stripe payment intent {PaymentIntentId}.", paymentIntent.Id);
                return true;
            }

            enrollment.Status = status.Value;
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation(
                "Enrollment {EnrollmentId} status updated to {EnrollmentStatus} from Stripe event {EventType}.",
                enrollment.Id,
                enrollment.Status,
                stripeEvent.Type);

            return true;
        }
    }
}
