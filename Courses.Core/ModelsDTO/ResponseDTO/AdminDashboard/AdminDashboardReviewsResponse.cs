namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminDashboardReviewsResponse
    {
        public int Id { get; set; } // RatingId
        public int Rating { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
