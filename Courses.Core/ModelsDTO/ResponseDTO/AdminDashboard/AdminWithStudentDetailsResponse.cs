
using Courses.Core.Models.ApplicationUsers;

namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminWithStudentDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public int NumberOfEnrollments { get; set; }
        public int Age { get; set; }

        public string UserId { get; set; }
        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public string Address { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public List<AdminEnrollmentsWithStudentResponse> Enrollments { get; set; } = new();
    }
}
