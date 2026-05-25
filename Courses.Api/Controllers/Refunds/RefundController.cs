using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Refunds;
using Courses.Core.ModelsDTO.ResponseDTO.Refunds;
using Courses.Core.Services.Contract.RefundsServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Refunds
{
    [Authorize(Roles = Roles.Student)]
    public class RefundController : BaseController
    {
        protected readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        #region Create Refund Async
        [HttpPost("CreateRefund")] // POST: /api/Refund/CreateRefund
        public async Task<ActionResult<ApplicationServiceResult<RefundResponse>>> CreateRefundAsync(RefundRequest req)
        {
            var result = await _refundService.CreateRefundRequestAsync(req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400));

            return Ok(result);
        }
        #endregion
    }
}
