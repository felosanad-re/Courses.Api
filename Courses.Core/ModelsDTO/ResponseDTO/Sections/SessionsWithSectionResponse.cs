
namespace Courses.Core.ModelsDTO.ResponseDTO.Sections
{
    public class SessionsWithSectionResponse
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string ZoomMeetingId { get; set; }
        public int Order { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; } // Session Time
        public string Status { get; set; }
        public string? RecordingUrl { get; set; }
    }
}
