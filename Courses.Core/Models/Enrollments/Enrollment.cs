using Courses.Core.Models.Courses;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;

namespace Courses.Core.Models.Enrollments
{
    /// <summary>
    /// Join entity representing a student's enrollment in a course.
    /// Tracks enrollment date, completion status, and overall progress percentage.
    /// </summary>
    public class Enrollment : BaseModel
    {

        // The student who enrolled (many-to-one)
        public int StudentId { get; set; }
        public Student Student { get; set; }

        // The course the student enrolled in (many-to-one)
        public int CourseId { get; set; }
        public Course Course { get; set; }

        // Whether the student has completed all course requirements
        public bool IsCompleted { get; set; }

        // For Paid Courses
        public bool IsPaid { get; set; }
        public decimal Amount { get; set; }

        // For Refused Course
        public string? CancellationReason { get; set; }

        // Overall progress percentage (0-100), can be calculated from StudentLectureProgress
        public decimal Progress { get; set; }

        public EnrollStatus Status { get; set; }

        public string? PaymentIntentId { get; set; }

        // Tracks which individual lectures this student has completed within the course
        public ICollection<StudentLectureProgress> LectureProgresses { get; set; } = new HashSet<StudentLectureProgress>();
    }
}
