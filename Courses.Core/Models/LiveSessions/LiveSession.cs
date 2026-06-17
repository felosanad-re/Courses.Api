using Courses.Core.Models.Courses;

namespace Courses.Core.Models.LiveSessions
{
    public class LiveSession : BaseModel
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public string ZoomMeetingId { get; set; }
        public string HostJoinUrl { get; set; } // Instructor Only
        public string StudentJoinUrl { get; set; } // For Students
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; } // Session Time
        public LiveSessionStatus Status { get; set; }
        public string? RecordingUrl { get; set; }
    }
}
