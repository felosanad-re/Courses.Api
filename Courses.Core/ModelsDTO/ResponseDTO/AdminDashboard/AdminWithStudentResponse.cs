namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminWithStudentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public int NumberOfEnrollments { get; set; }
        public int Age { get; set; }

        public string UserId { get; set; }

        public bool IsInstructor { get; set; }
    }
}
