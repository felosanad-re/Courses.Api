namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminInstructorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public int NumberOfCourses { get; set; }
        public int Age { get; set; }

        public string UserId { get; set; }
    }
}
