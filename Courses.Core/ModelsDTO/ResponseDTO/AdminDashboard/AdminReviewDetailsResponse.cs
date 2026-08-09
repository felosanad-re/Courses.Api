namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminReviewDetailsResponse
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Image { get; set; }
        public string CourseName { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
