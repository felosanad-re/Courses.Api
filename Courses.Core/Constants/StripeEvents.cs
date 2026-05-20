using Stripe;

namespace Courses.Core.Constants
{
    public static class StripeEvents
    {
        public static string PaymentIntentSucceeded = "payment_intent.succeeded";
        public static string PaymentIntentPaymentFailed = "payment_intent.payment_failed";
        public static string PaymentIntentPaymentCanceled = "payment_intent.canceled";
        public static string RefundCreated = "refund.created";
        public static string RefundFailed = "refund.failed";
    }
}
