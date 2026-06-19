namespace Courses.Core.ModelsDTO.ResponseDTO.LiveSessions
{
    public class LiveSessionListResponse
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; }
    }
}
