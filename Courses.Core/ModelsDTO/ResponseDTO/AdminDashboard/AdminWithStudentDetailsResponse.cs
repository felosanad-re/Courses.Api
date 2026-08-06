using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;

namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminWithStudentDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public int NumberOfEnrollments { get; set; }
        public int Age { get; set; }

        public List<AdminEnrollmentsWithStudentResponse> Enrollments { get; set; } = new();
    }
}
