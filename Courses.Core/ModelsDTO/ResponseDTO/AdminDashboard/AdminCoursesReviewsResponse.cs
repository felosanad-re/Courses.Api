namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminCoursesReviewsResponse
    {
        public int Id { get; set; } // Review
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Image { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public decimal AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}
