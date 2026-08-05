using Courses.Core.Models.Courses;

namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminInstructorCoursesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }

        public string CourseCategoryName { get; set; }
        public int CourseCategoryId { get; set; }

        public string Type { get; set; }

        // explain if the course is published or still in review
        public string Status { get; set; }
        public DateTime? PublishedAt { get; set; }

        public decimal AverageRating { get; set; }
        public int RatingCount { get; set; }

        public int NumberOfSections { get; set; }
    }
}
