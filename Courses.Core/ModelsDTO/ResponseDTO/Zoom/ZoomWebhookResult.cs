namespace Courses.Core.ModelsDTO.ResponseDTO.Zoom
{
    /// <summary>
    /// The result returned by <see cref="Services.Contract.ZoomServices.IZoomWebhookService.HandleWebhookAsync"/>.
    /// Carries the HTTP success flag plus an optional JSON body that must be sent back to Zoom
    /// (e.g. the encrypted token required during endpoint URL validation).
    /// </summary>
    public class ZoomWebhookResult
    {
        /// <summary>
        /// <c>true</c> when the request was verified and acknowledged;
        /// <c>false</c> when verification failed (caller should return 401).
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Optional JSON body to return to Zoom. Used during <c>endpoint.url_validation</c>
        /// where Zoom expects <c>{ "plainToken": "...", "encryptedToken": "..." }</c>.
        /// </summary>
        public string? ResponseBody { get; set; }
    }
}
