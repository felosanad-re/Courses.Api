namespace Courses.Core.ModelsDTO.ResponseDTO.Progress
{
    public class ProgressWithLectureResponse
    {
        public int EnrollmentId { get; set; }

        public int LectureId { get; set; }
        public string LectureName { get; set; }

        public double LastWatchedSeconds { get; set; } = 0;

        public double VideoDuration { get; set; } = 0;

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
    }
}
