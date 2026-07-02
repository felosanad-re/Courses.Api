using Courses.Core.Services.Contract.ZoomServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.ZoomWebHooks
{
    // The webhook endpoint is called by Zoom (server-to-server), so there is no
    // authenticated user and no JWT token. Authenticity is verified inside the
    // service via the x-zm-signature header + the shared WebhookSecret.
    [AllowAnonymous]
    public class ZoomWebHooksController : BaseController
    {
        protected readonly IZoomWebhookService _zoomWebhookService;
        protected readonly ILogger<ZoomWebHooksController> _logger;

        public ZoomWebHooksController(IZoomWebhookService zoomWebhookService, ILogger<ZoomWebHooksController> logger)
        {
            _zoomWebhookService = zoomWebhookService;
            _logger = logger;
        }

        // Zoom sends the event payload in the raw request body plus three
        // verification headers: the Secret Token, the HMAC signature, and a timestamp.
        [HttpPost("webhook")] // POST: /api/ZoomWebHooks/webhook
        public async Task<IActionResult> Handle()
        {
            // DIAGNOSTIC: log the request scheme + path so we can confirm Zoom is hitting HTTPS directly
            // (if Zoom sends to http:// and UseHttpsRedirection kicks in, the validation response body is lost).
            _logger.LogInformation("Zoom webhook received: scheme={Scheme}, host={Host}, path={Path}, method={Method}",
                Request.Scheme, Request.Host.Value, Request.Path.Value, Request.Method);

            // Read the raw JSON body exactly as Zoom sent it.
            // We must NOT use model binding here because the signature is computed
            // over the raw bytes; any re-serialization would break verification.
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            // DIAGNOSTIC: log the raw payload length + first 200 chars to confirm the body is not empty.
            _logger.LogInformation("Zoom webhook payload length={Length}, preview={Preview}",
                payload.Length, payload.Length > 200 ? payload.Substring(0, 200) : payload);

            // Zoom verification headers:
            //   x-zm-webhook-token      -> Secret Token (simple comparison)
            //   x-zm-signature          -> HMAC-SHA256 signature "v0=<hex>"
            //   x-zm-request-timestamp  -> timestamp used in the HMAC message
            var webhookToken = Request.Headers["x-zm-webhook-token"].ToString();
            var signature = Request.Headers["x-zm-signature"].ToString();
            var timestamp = Request.Headers["x-zm-request-timestamp"].ToString();

            // DIAGNOSTIC: log which verification headers Zoom actually sent.
            _logger.LogInformation("Zoom webhook headers: tokenPresent={TokenPresent}, signaturePresent={SigPresent}, timestampPresent={TsPresent}",
                !string.IsNullOrWhiteSpace(webhookToken), !string.IsNullOrWhiteSpace(signature), !string.IsNullOrWhiteSpace(timestamp));

            // Delegate to the service: verify authenticity + dispatch the event.
            var result = await _zoomWebhookService.HandleWebhookAsync(payload, webhookToken, signature, timestamp);

            // IsValid = false => verification failed (or secret not configured) -> 401.
            // IsValid = true  => event acknowledged (processed or intentionally ignored) -> 200.
            // Returning 200 tells Zoom to stop retrying the event.
            if (!result.IsValid)
            {
                _logger.LogWarning("Zoom webhook rejected (401): verification failed.");
                return Unauthorized();
            }

            // During endpoint.url_validation, Zoom expects a JSON body with the encrypted token.
            // For all other events, an empty 200 is enough.
            if (!string.IsNullOrEmpty(result.ResponseBody))
            {
                // DIAGNOSTIC: log the exact response body + content-type we are about to send to Zoom.
                _logger.LogInformation("Zoom webhook returning JSON body (200): {ResponseBody}", result.ResponseBody);
                return Content(result.ResponseBody, "application/json");
            }

            _logger.LogInformation("Zoom webhook returning empty 200 (acknowledged).");
            return Ok();
        }
    }
}
