
namespace Courses.Core.ModelsDTO.ResponseDTO.LiveSessions
{
    public class LiveSessionResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string ZoomMeetingId { get; set; }
        public string HostJoinUrl { get; set; } // Instructor Only
        public string StudentJoinUrl { get; set; } // For Students
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; } // Session Time
        public string Status { get; set; }
        public string? RecordingUrl { get; set; }
    }
}
