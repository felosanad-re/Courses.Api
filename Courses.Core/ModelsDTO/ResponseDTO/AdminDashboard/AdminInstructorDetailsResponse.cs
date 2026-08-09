namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminInstructorDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public int NumberOfCourses { get; set; }
        public int Age { get; set; }

        public string UserId { get; set; }
        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public string Address { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public int PhoneNumber { get; set; }


        // NOTE:
        // For portfolio simplicity, instructor courses are returned with the details response.
        // In production, this should be replaced by a separate paginated endpoint.
        public List<AdminInstructorCoursesResponse> Courses { get; set; } = new();
    }
}
