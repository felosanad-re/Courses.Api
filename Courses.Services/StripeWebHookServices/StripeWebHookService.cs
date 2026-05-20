using Courses.Core.Constants;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Options;
using Courses.Core.Services.Contract.EnrollmentServices;
using Courses.Core.Services.Contract.RefundsServices;
using Courses.Core.Services.Contract.StripeWebHookServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace Courses.Services.StripeWebHookServices
{
    public class StripeWebHookService : IStripeWebHookService
    {
        #region Stripe Events
        private static readonly IReadOnlyDictionary<string, EnrollStatus> PaymentIntentFinalStatuses =
            new Dictionary<string, EnrollStatus>
            {
                { StripeEvents.PaymentIntentSucceeded, EnrollStatus.Active },
                { StripeEvents.PaymentIntentPaymentFailed, EnrollStatus.PaymentFailed },
                { StripeEvents.PaymentIntentPaymentCanceled, EnrollStatus.PaymentCancelled }
            };

        private static readonly IReadOnlySet<string> RefundEvents =
            new HashSet<string>
            {
                StripeEvents.RefundCreated,
                StripeEvents.RefundFailed,
            };
        #endregion

        #region DI Service
        private readonly IEnrollmentService _enrollmentService;
        private readonly IRefundService _refundService;
        private readonly ILogger<StripeWebHookService> _logger;
        private readonly StripeOptions _stripeOptions;

        public StripeWebHookService(
            IEnrollmentService enrollmentService,
            ILogger<StripeWebHookService> logger,
            IOptions<StripeOptions> stripeOptions,
            IRefundService refundService)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
            _stripeOptions = stripeOptions.Value;
            _refundService = refundService;
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

            if (PaymentIntentFinalStatuses.TryGetValue(stripeEvent.Type, out var paymentStatus))
                return await HandlePaymentIntentEventAsync(stripeEvent, paymentStatus);

            if (RefundEvents.Contains(stripeEvent.Type))
                return await HandleRefundEventAsync(stripeEvent);

            _logger.LogInformation("Stripe webhook event {EventType} ignored.", stripeEvent.Type);
            return true;
        }
        #endregion

        #region Helper Methods

        private async Task<bool> HandlePaymentIntentEventAsync(Event stripeEvent, EnrollStatus status)
        {
            // If Event Is not PaymentIntent
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

        private async Task<bool> HandleRefundEventAsync(Event stripeEvent)
        {
            if (stripeEvent.Data.Object is Refund refund)
            {
                var status = MapStripeRefundStatusToEnrollmentStatus(refund.Status, stripeEvent.Type);
                return await UpdateRefundStatusAsync(refund.PaymentIntentId, status, stripeEvent.Type);
            }

            _logger.LogInformation("Stripe webhook event {EventType} ignored because object is not a refund.", stripeEvent.Type);
            return true;
        }

        private async Task<bool> UpdateRefundStatusAsync(string? paymentIntentId, EnrollStatus status, string eventType)
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId))
            {
                _logger.LogWarning("Stripe webhook event {EventType} ignored because payment intent id is missing.", eventType);
                return true;
            }

            var updateResult = await _refundService.UpdateRefundStatusAsync(paymentIntentId, status);

            if (!updateResult.Succeed)
            {
                _logger.LogWarning(
                    "Stripe webhook event {EventType} could not update refund status for payment intent {PaymentIntentId}: {Message}",
                    eventType,
                    paymentIntentId,
                    updateResult.Message);

                return true;
            }

            _logger.LogInformation(
                "Enrollment {EnrollmentId} refund status updated to {EnrollmentStatus} from Stripe event {EventType}.",
                updateResult.Data?.EnrollmentId,
                updateResult.Data?.Status,
                eventType);

            return true;
        }

        private static EnrollStatus MapStripeRefundStatusToEnrollmentStatus(string? stripeRefundStatus, string eventType)
        {
            if (eventType == "refund.failed")
                return EnrollStatus.Active;

            return stripeRefundStatus switch
            {
                "succeeded" => EnrollStatus.Refunded,
                "failed" => EnrollStatus.Active,
                "canceled" => EnrollStatus.Active,
                _ => EnrollStatus.RefundedPending
            };
        }

        #endregion
    }
}
