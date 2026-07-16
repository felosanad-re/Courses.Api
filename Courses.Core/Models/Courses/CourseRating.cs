using Courses.Core.Models.Students;

namespace Courses.Core.Models.Courses
{
    public class CourseRating : BaseModel
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
