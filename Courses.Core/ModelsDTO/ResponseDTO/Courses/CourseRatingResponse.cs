namespace Courses.Core.ModelsDTO.ResponseDTO.Courses
{
    public class CourseRatingResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string StudentName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
