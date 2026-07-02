namespace Courses.Core.Options
{
    public class ZoomOptions
    {
        public const string SectionName = "Zoom";

        public string AccountId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// The Secret Token configured in the Zoom webhook endpoint.
        /// Used to verify the <c>x-zm-signature</c> header of incoming webhook requests.
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
