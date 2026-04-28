namespace Courses.Core.Models.Enrollments
{
    /// <summary>
    /// Tracks whether a student has completed a specific lecture.
    /// This is the granular progress tracking that feeds into Enrollment.Progress.
    /// Without this, you can't know which lectures a student has watched.
    /// </summary>
    public class StudentLectureProgress
    {
        public int Id { get; set; }

        // The enrollment this progress belongs to (many-to-one)
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }

        //The lecture being tracked (many-to-one
        public int LectureId { get; set; }
        public Lecture Lecture { get; set; }

        // Whether the student has completed this lecture
        public bool IsCompleted { get; set; }

        // When the student completed this lecture (null if not yet completed)
        public DateTime? CompletedAt { get; set; }
    }
}
