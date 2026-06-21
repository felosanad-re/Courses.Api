using Courses.Core.Models.Enrollments;

namespace Courses.Core.Models.LiveSessions
{
    public class LiveSession : BaseModel
    {
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public string Topic { get; set; }
        public string ZoomMeetingId { get; set; }
        public int Order { get; set; }
        public string HostJoinUrl { get; set; } // Instructor Only
        public string StudentJoinUrl { get; set; } // For Students
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; } // Session Time
        public LiveSessionStatus Status { get; set; }
        public string? RecordingUrl { get; set; }
    }
}
