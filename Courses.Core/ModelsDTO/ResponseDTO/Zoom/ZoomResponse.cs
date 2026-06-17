using Newtonsoft.Json;

namespace Courses.Core.ModelsDTO.ResponseDTO.Zoom
{
    public class ZoomResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("join_url")]
        public string JoinUrl { get; set; } // For Students
        [JsonProperty("start_url")]
        public string StartUrl { get; set; } // For Instructor

        [JsonProperty("topic")]
        public string Topic { get; set; }
    }
}
