using Courses.Core.Models.Enrollments;

namespace Courses.Core.ModelsDTO.ResponseDTO.StudentLectureProgress
{
    public class StudentLectureProgressResponse
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }

        public int LectureId { get; set; }
        public string LectureName { get; set; } // NFP


        // Whether the student has completed this lecture
        public bool IsCompleted { get; set; }

        // When the student completed this lecture (null if not yet completed)
        public DateTime? CompletedAt { get; set; }
    }
}
