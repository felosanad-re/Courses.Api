namespace Courses.Core.ModelsDTO.RequestDTO.Courses
{
    public class CourseProgressResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int TotalLectures { get; set; }
        public int CompletedLectures { get; set; }
        public double ProgressPercentage { get; set; }
    }
}
