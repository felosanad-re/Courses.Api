namespace Courses.Core.Services.Contract.StripeWebHookServices
{
    public interface IStripeWebHookService
    {
        Task<bool> WebHook(string json, string stripeSignature);
    }
}
