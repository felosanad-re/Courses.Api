using System.Security.Cryptography;
using System.Text;
using Courses.Core.Constants;
using Courses.Core.Models.LiveSessions;
using Courses.Core.ModelsDTO.ResponseDTO.Zoom;
using Courses.Core.Options;
using Courses.Core.Services.Contract.LiveSessionServices;
using Courses.Core.Services.Contract.ZoomServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Courses.Services.ZoomServices
{
    public class ZoomWebhookService : IZoomWebhookService
    {
        #region DI Services
        protected readonly ZoomOptions _zoomOptions;
        protected readonly ILiveSessionService _liveSessionService;
        protected readonly ILogger<ZoomWebhookService> _logger;

        public ZoomWebhookService(
            IOptions<ZoomOptions> zoomOptions,
            ILiveSessionService liveSessionService,
            ILogger<ZoomWebhookService> logger)
        {
            _zoomOptions = zoomOptions.Value;
            _liveSessionService = liveSessionService;
            _logger = logger;
        }
        #endregion

        #region Handle Webhook
        /// <summary>
        /// Entry point for every Zoom webhook notification.
        /// Flow:
        ///   1) Verify the request authenticity using the shared <see cref="ZoomOptions.WebhookSecret"/>.
        ///   2) Parse the event type from the JSON payload.
        ///   3) Dispatch to the matching handler (endpoint.url_validation / meeting.started / meeting.ended / recording.completed).
        ///   4) Return a <see cref="ZoomWebhookResult"/> so the controller answers Zoom with HTTP 200 (acknowledged),
        ///      optionally carrying a response body (needed for endpoint validation).
        /// </summary>
        public async Task<ZoomWebhookResult> HandleWebhookAsync(string payload, string? webhookToken, string? signature, string? timestamp)
        {
            var result = new ZoomWebhookResult();

            // STEP 1: Make sure the webhook secret is configured.
            // Without it we cannot verify any request, so we reject everything.
            if (string.IsNullOrWhiteSpace(_zoomOptions.WebhookSecret))
            {
                _logger.LogError("Zoom webhook secret is not configured.");
                return result; // IsValid = false
            }

            // STEP 2: Verify the request authenticity.
            // Zoom supports two verification mechanisms; we accept either one:
            //   (a) Secret Token  -> x-zm-webhook-token header compared to the configured secret.
            //   (b) HMAC signature -> x-zm-signature (v0=<hex>) computed over "v0:{timestamp}:{payload}".
            // If BOTH fail, the request is NOT from Zoom -> reject it (controller returns 401).
            if (!VerifyWebhookToken(webhookToken) && !VerifyHmacSignature(payload, signature, timestamp))
            {
                _logger.LogWarning("Invalid Zoom webhook: token and signature verification both failed.");
                return result; // IsValid = false
            }

            result.IsValid = true;

            // STEP 3: Parse the JSON payload to read the event type and the meeting id.
            // Zoom payload structure (simplified):
            //   { "event": "meeting.started", "payload": { "object": { "id": "123456789", ... } } }
            JObject webhookEvent;
            try
            {
                webhookEvent = JObject.Parse(payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Zoom webhook payload is not valid JSON.");
                result.IsValid = false; // Malformed payload -> reject.
                return result;
            }

            var eventType = webhookEvent["event"]?.ToString();
            var meetingId = webhookEvent["payload"]?["object"]?["id"]?.ToString();

            // STEP 4: Dispatch to the matching handler based on the event type.
            // Unknown events are intentionally acknowledged (IsValid stays true) so Zoom does not retry them,
            // exactly like the Stripe webhook service does.
            switch (eventType)
            {
                case ZoomEvents.EndpointUrlValidation:
                    result.ResponseBody = HandleUrlValidation(webhookEvent);
                    break;

                case ZoomEvents.MeetingStarted:
                    await HandleMeetingStartedAsync(meetingId);
                    break;

                case ZoomEvents.MeetingEnded:
                    await HandleMeetingEndedAsync(meetingId);
                    break;

                case ZoomEvents.RecordingCompleted:
                    await HandleRecordingCompletedAsync(webhookEvent, meetingId);
                    break;

                default:
                    _logger.LogInformation("Zoom webhook event {EventType} ignored.", eventType);
                    break;
            }

            // Acknowledge the event so Zoom stops retrying it.
            return result;
        }
        #endregion

        #region Endpoint URL Validation
        /// <summary>
        /// Handles the <c>endpoint.url_validation</c> event sent by Zoom when enabling a webhook.
        /// Zoom sends a <c>plainToken</c> and expects back a JSON body containing the same
        /// <c>plainToken</c> plus an <c>encryptedToken</c> = HMAC-SHA256(plainToken) using the secret.
        /// </summary>
        private string HandleUrlValidation(JObject webhookEvent)
        {
            // Zoom payload: { "event": "endpoint.url_validation", "payload": { "plainToken": "..." } }
            var plainToken = webhookEvent["payload"]?["plainToken"]?.ToString();

            if (string.IsNullOrWhiteSpace(plainToken))
            {
                _logger.LogWarning("Zoom endpoint.url_validation ignored: missing plainToken.");
                return string.Empty;
            }

            // Compute the encrypted token: HMAC-SHA256 of the plainToken using the webhook secret.
            // Zoom expects the resulting hash encoded as lowercase hex.
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_zoomOptions.WebhookSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainToken));
            var encryptedToken = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            // Build the response body Zoom expects:
            //   { "plainToken": "...", "encryptedToken": "..." }
            var response = new
            {
                plainToken,
                encryptedToken
            };

            _logger.LogInformation("Zoom endpoint.url_validation handled successfully. plainToken={PlainToken}, encryptedToken={EncryptedToken}", plainToken, encryptedToken);

            return JsonConvert.SerializeObject(response);
        }
        #endregion

        #region Verification: Secret Token
        /// <summary>
        /// Verifies the <c>x-zm-webhook-token</c> header against the configured
        /// <see cref="ZoomOptions.WebhookSecret"/> using a constant-time comparison.
        /// This is the simplest verification method offered by Zoom.
        /// </summary>
        private bool VerifyWebhookToken(string? webhookToken)
        {
            if (string.IsNullOrWhiteSpace(webhookToken))
                return false;

            // Constant-time comparison to avoid timing attacks.
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(webhookToken),
                Encoding.UTF8.GetBytes(_zoomOptions.WebhookSecret));
        }
        #endregion

        #region Verification: HMAC Signature
        /// <summary>
        /// Verifies the <c>x-zm-signature</c> header using HMAC-SHA256.
        /// Zoom computes the hash over the message: <c>v0:{timestamp}:{payload}</c>
        /// using the shared secret, and sends it as <c>v0=<hex></c>.
        /// </summary>
        private bool VerifyHmacSignature(string payload, string? signature, string? timestamp)
        {
            if (string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(timestamp))
                return false;

            // Zoom signature format: "v0=<hex>".
            // We strip the "v0=" prefix to compare only the hash part.
            var receivedHash = signature.StartsWith("v0=", StringComparison.OrdinalIgnoreCase)
                ? signature.Substring(3)
                : signature;

            // The HMAC message MUST include the timestamp exactly as Zoom sent it:
            //   "v0:{timestamp}:{payload}"
            var message = $"v0:{timestamp}:{payload}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_zoomOptions.WebhookSecret));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            var computedHex = BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();

            // Constant-time comparison to avoid timing attacks.
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedHex),
                Encoding.UTF8.GetBytes(receivedHash.ToLowerInvariant()));
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles <c>meeting.started</c>: marks the related live session as <see cref="LiveSessionStatus.Live"/>.
        /// </summary>
        private async Task HandleMeetingStartedAsync(string? meetingId)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
            {
                _logger.LogWarning("Zoom meeting.started event ignored: missing meeting id.");
                return;
            }

            var result = await _liveSessionService.UpdateStatusByZoomMeetingIdAsync(meetingId, LiveSessionStatus.Live);

            if (!result.Succeed)
                _logger.LogWarning("Zoom meeting.started could not update session for meeting {MeetingId}: {Message}", meetingId, result.Message);
            else
                _logger.LogInformation("Live session set to Live from Zoom meeting.started for meeting {MeetingId}.", meetingId);
        }

        /// <summary>
        /// Handles <c>meeting.ended</c>: marks the related live session as <see cref="LiveSessionStatus.Ended"/>.
        /// </summary>
        private async Task HandleMeetingEndedAsync(string? meetingId)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
            {
                _logger.LogWarning("Zoom meeting.ended event ignored: missing meeting id.");
                return;
            }

            var result = await _liveSessionService.UpdateStatusByZoomMeetingIdAsync(meetingId, LiveSessionStatus.Ended);

            if (!result.Succeed)
                _logger.LogWarning("Zoom meeting.ended could not update session for meeting {MeetingId}: {Message}", meetingId, result.Message);
            else
                _logger.LogInformation("Live session set to Ended from Zoom meeting.ended for meeting {MeetingId}.", meetingId);
        }

        /// <summary>
        /// Handles <c>recording.completed</c>: stores the recording share URL on the related live session.
        /// Zoom sends the recording files inside <c>payload.object.recording_files</c>; we pick the first
        /// file that exposes a <c>share_url</c> (or fall back to <c>play_url</c>).
        /// </summary>
        private async Task HandleRecordingCompletedAsync(JObject webhookEvent, string? meetingId)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
            {
                _logger.LogWarning("Zoom recording.completed event ignored: missing meeting id.");
                return;
            }

            // Extract the recording url from the payload.
            var recordingUrl = ExtractRecordingUrl(webhookEvent);
            if (string.IsNullOrWhiteSpace(recordingUrl))
            {
                _logger.LogWarning("Zoom recording.completed event ignored: no recording url found for meeting {MeetingId}.", meetingId);
                return;
            }

            var result = await _liveSessionService.SaveRecordingUrlByZoomMeetingIdAsync(meetingId, recordingUrl);

            if (!result.Succeed)
                _logger.LogWarning("Zoom recording.completed could not save recording url for meeting {MeetingId}: {Message}", meetingId, result.Message);
            else
                _logger.LogInformation("Recording url saved from Zoom recording.completed for meeting {MeetingId}.", meetingId);
        }

        /// <summary>
        /// Reads the recording share/play url from the Zoom webhook payload.
        /// </summary>
        private static string? ExtractRecordingUrl(JObject webhookEvent)
        {
            // recording_files is an array; we take the first available url.
            var recordingFiles = webhookEvent["payload"]?["object"]?["recording_files"] as JArray;
            if (recordingFiles is null || recordingFiles.Count == 0)
                return null;

            foreach (var file in recordingFiles)
            {
                var shareUrl = file["share_url"]?.ToString();
                if (!string.IsNullOrWhiteSpace(shareUrl))
                    return shareUrl;

                var playUrl = file["play_url"]?.ToString();
                if (!string.IsNullOrWhiteSpace(playUrl))
                    return playUrl;
            }

            return null;
        }

        #endregion
    }
}
