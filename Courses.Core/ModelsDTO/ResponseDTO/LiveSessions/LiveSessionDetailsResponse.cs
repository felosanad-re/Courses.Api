namespace Courses.Core.ModelsDTO.ResponseDTO.LiveSessions
{
    public class LiveSessionDetailsResponse
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string Topic { get; set; }
        public string ZoomMeetingId { get; set; }
        public string HostJoinUrl { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; }
        public string? RecordingUrl { get; set; }
    }
}
