using Courses.Core.Models.Courses;
using Courses.Core.Models.Students;

namespace Courses.Core.Models.Enrollments
{
    /// <summary>
    /// Join entity representing a student's enrollment in a course.
    /// Tracks enrollment date, completion status, and overall progress percentage.
    /// </summary>
    public class Enrollment
    {
        public int Id { get; set; }

        // The student who enrolled (many-to-one)
        public int StudentId { get; set; }
        public Student Student { get; set; }

        // The course the student enrolled in (many-to-one)
        public int CourseId { get; set; }
        public Course Course { get; set; }

        // When the student enrolled
        public DateTime EnrolledAt { get; set; }

        // Whether the student has completed all course requirements
        public bool IsCompleted { get; set; }

        // Overall progress percentage (0-100), can be calculated from StudentLectureProgress
        public decimal Progress { get; set; }

        // Tracks which individual lectures this student has completed within the course
        public ICollection<StudentLectureProgress> LectureProgresses { get; set; } = new HashSet<StudentLectureProgress>();
    }
}
