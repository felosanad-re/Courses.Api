using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.ResponseDTO.StudentLectureProgress;

namespace Courses.Core.ModelsDTO.ResponseDTO.Enrollment
{
    public class EnrollmentResponse
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } // NFP

        public int CourseId { get; set; }
        public string CourseName { get; set; } // NFP

        public DateTime EnrolledAt { get; set; }

        public bool IsCompleted { get; set; }

        public decimal Progress { get; set; }

        public List<StudentLectureProgressResponse> LectureProgresses { get; set; }
    }
}
