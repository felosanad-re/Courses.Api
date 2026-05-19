using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Options;
using Courses.Core.Services.Contract.EnrollmentServices;
using Courses.Core.Services.Contract.StripeWebHookServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace Courses.Services.StripeWebHookServices
{
    public class StripeWebHookService : IStripeWebHookService
    {
        private static readonly IReadOnlyDictionary<string, EnrollStatus> PaymentIntentFinalStatuses =
            new Dictionary<string, EnrollStatus>
            {
                { "payment_intent.succeeded", EnrollStatus.PaymentSucceeded },
                { "payment_intent.payment_failed", EnrollStatus.PaymentFailed },
                { "payment_intent.canceled", EnrollStatus.PaymentCancelled }
            };

        #region DI Service
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<StripeWebHookService> _logger;
        private readonly StripeOptions _stripeOptions;

        public StripeWebHookService(
            IEnrollmentService enrollmentService,
            ILogger<StripeWebHookService> logger,
            IOptions<StripeOptions> stripeOptions)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
            _stripeOptions = stripeOptions.Value;
        }
        #endregion

        #region WebHook
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

            if (!PaymentIntentFinalStatuses.TryGetValue(stripeEvent.Type, out var status))
            {
                _logger.LogInformation("Stripe webhook event {EventType} ignored before payment final status.", stripeEvent.Type);
                return true;
            }

            if (stripeEvent.Data.Object is not PaymentIntent paymentIntent)
            {
                _logger.LogInformation("Stripe webhook event {EventType} ignored because object is not a payment intent.", stripeEvent.Type);
                return true;
            }

            var updateResult = await _enrollmentService.UpdateEnrollmentStatusAsync(paymentIntent.Id, status);

            if (!updateResult.Succeed)
            {
                _logger.LogWarning(
                    "Stripe webhook event {EventType} could not update enrollment for payment intent {PaymentIntentId}: {Message}",
                    stripeEvent.Type,
                    paymentIntent.Id,
                    updateResult.Message);

                return true;
            }

            _logger.LogInformation(
                "Enrollment {EnrollmentId} status updated to {EnrollmentStatus} from Stripe event {EventType}.",
                updateResult.Data?.EnrollmentId,
                updateResult.Data?.Status,
                stripeEvent.Type);

            return true;
        }
        #endregion
    }
}
