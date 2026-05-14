namespace Courses.Core.ModelsDTO.ResponseDTO.Lectures
{
    public class CourseWithLectureVideoResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public int Order { get; set; }
    }
}
