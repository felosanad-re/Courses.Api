namespace Courses.Core.Constants
{
    /// <summary>
    /// Zoom webhook event types that the application is interested in handling.
    /// Mirrors the <c>event</c> field sent by Zoom in the webhook payload.
    /// </summary>
    public static class ZoomEvents
    {
        /// <summary>
        /// Sent by Zoom when enabling a webhook endpoint to verify the URL is reachable
        /// and can echo back the encrypted token. Must be handled before the webhook
        /// can be activated in the Zoom dashboard.
        /// </summary>
        public const string EndpointUrlValidation = "endpoint.url_validation";

        /// <summary>
        /// Fired when a meeting starts. Used to flip the related live session
        /// status from <c>Scheduled</c> to <c>Live</c>.
        /// </summary>
        public const string MeetingStarted = "meeting.started";

        /// <summary>
        /// Fired when a meeting ends. Used to flip the related live session
        /// status from <c>Live</c> to <c>Ended</c>.
        /// </summary>
        public const string MeetingEnded = "meeting.ended";

        /// <summary>
        /// Fired when a cloud recording finishes processing.
        /// Used to persist the recording share URL on the related live session.
        /// </summary>
        public const string RecordingCompleted = "recording.completed";
    }
}
