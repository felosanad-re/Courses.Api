using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Payments;
using Courses.Core.ModelsDTO.ResponseDTO.Payments;
using Courses.Core.Services.Contract.PaymentsServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Payment
{
    public class PaymentController : BaseController
    {
        protected readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        #region Create Payment
        [HttpPost("CreatePaymentIntent")] // POST: /api/Payment/CreatePaymentIntent
        public async Task<ActionResult<ApplicationServiceResult<PaymentResponse>>> CreatePayment(PaymentRequest req)
        {
            var result = await _paymentService.CreatePaymentIntent(req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Get Payment
        [HttpGet("PaymentIntent")] // GET: /api/Payment/PaymentIntent
        public async Task<ActionResult<ApplicationServiceResult<PaymentResponse>>> GetPayment(string paymentIntentId)
        {
            var result = await _paymentService.GetPaymentIntent(paymentIntentId);
            if(!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion
    }
}
