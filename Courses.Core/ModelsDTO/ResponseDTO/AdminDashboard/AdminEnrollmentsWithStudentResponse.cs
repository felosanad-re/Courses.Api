using Courses.Core.Models.Courses;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;

namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminEnrollmentsWithStudentResponse
    {
        public int Id { get; set; } // Enrollment
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        // The course the student enrolled in (many-to-one)
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        // For Paid Courses
        public bool IsPaid { get; set; }
        public decimal Amount { get; set; }

        public string Status { get; set; }
    }
}
