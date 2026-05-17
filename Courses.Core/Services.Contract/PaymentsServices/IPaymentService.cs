using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Payments;
using Courses.Core.ModelsDTO.ResponseDTO.Payments;

namespace Courses.Core.Services.Contract.PaymentsServices
{
    public interface IPaymentService
    {
        // Create PaymentIntent
        Task<ApplicationServiceResult<PaymentResponse>> CreatePaymentIntent(PaymentRequest req);

        // Get PaymentIntent
        Task<ApplicationServiceResult<PaymentResponse>> GetPaymentIntent(string paymentIntentId);
    }
}
