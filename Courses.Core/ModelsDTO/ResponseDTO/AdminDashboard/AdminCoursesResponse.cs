namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminCoursesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public int EnrollmentsCount { get; set; } // Num. of students

        public int InstructorId { get; set; }
        public string InstructorName { get; set; }

        public string CourseCategory { get; set; }
        public int CourseCategoryId { get; set; }
    }
}
