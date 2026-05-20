using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Refunds;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.ModelsDTO.ResponseDTO.Refunds;

namespace Courses.Core.Services.Contract.RefundsServices
{
    public interface IRefundService
    {
        // Create Refund Service
        Task<ApplicationServiceResult<RefundResponse>> CreateRefundRequestAsync(RefundRequest req);

        // Update Enrollment Status To Refund
        Task<ApplicationServiceResult<RefundResponse>> UpdateRefundStatusAsync(string paymentIntentId, EnrollStatus status);
    }
}
