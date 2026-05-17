using Courses.Core.Services.Contract.StripeWebHookServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.StripeWebHook
{
    [AllowAnonymous]
    public class StripeWebHookController : BaseController
    {
        protected readonly IStripeWebHookService _stripeWebHookService;

        public StripeWebHookController(IStripeWebHookService stripeWebHookService)
        {
            _stripeWebHookService = stripeWebHookService;
        }

        [HttpPost("webhook")] // POST: /api/StripeWebHook/webhook
        public async Task<IActionResult> Handle()
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

            var result = await _stripeWebHookService.WebHook(json, stripeSignature);
            if (!result) return BadRequest();

            return Ok();
        }
    }
}
