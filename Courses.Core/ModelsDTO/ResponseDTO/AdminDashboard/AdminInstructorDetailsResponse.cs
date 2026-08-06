namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminInstructorDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public int NumberOfCourses { get; set; }
        public int Age { get; set; }


        // NOTE:
        // For portfolio simplicity, instructor courses are returned with the details response.
        // In production, this should be replaced by a separate paginated endpoint.
        public List<AdminInstructorCoursesResponse> Courses { get; set; } = new();
    }
}
