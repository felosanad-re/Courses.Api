using Courses.Core.ModelsDTO.ResponseDTO.Zoom;

namespace Courses.Core.Services.Contract.ZoomServices
{
    public interface IZoomWebhookService
    {
        /// <summary>
        /// Receives and verifies an incoming Zoom webhook notification, then
        /// dispatches the event to the appropriate handler based on its type.
        /// <para>
        /// Handled events:
        /// <list type="bullet">
        /// <item><c>endpoint.url_validation</c> — returns the encrypted token Zoom needs to validate the endpoint.</item>
        /// <item><c>meeting.started</c> — marks the related live session as <see cref="Courses.Core.Models.LiveSessions.LiveSessionStatus.Live"/>.</item>
        /// <item><c>meeting.ended</c> — marks the related live session as <see cref="Courses.Core.Models.LiveSessions.LiveSessionStatus.Ended"/>.</item>
        /// <item><c>recording.completed</c> — stores the recording URL on the related live session.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="payload">The raw JSON request body sent by Zoom.</param>
        /// <param name="webhookToken">The value of the <c>x-zm-webhook-token</c> header (Secret Token verification).</param>
        /// <param name="signature">The value of the <c>x-zm-signature</c> header (HMAC verification, format: <c>v0=<hex></c>).</param>
        /// <param name="timestamp">The value of the <c>x-zm-request-timestamp</c> header (used in the HMAC message).</param>
        /// <returns>
        /// A <see cref="ZoomWebhookResult"/> whose <see cref="ZoomWebhookResult.IsValid"/> is <c>true</c> when the
        /// request was verified and acknowledged (caller returns 200, optionally with the response body);
        /// <c>false</c> when verification fails (caller returns 401).
        /// </returns>
        Task<ZoomWebhookResult> HandleWebhookAsync(string payload, string? webhookToken, string? signature, string? timestamp);
    }
}
