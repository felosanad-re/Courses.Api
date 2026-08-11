using Courses.Core.Models.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class RecordedCoursesWithSpec : BaseSpecifications<Course>
    {
        public RecordedCoursesWithSpec(int courseId)
            : base(x => x.Id == courseId && x.Type == CourseType.RecorderCourse)
        {
            Includes.Add(c => c.CourseCategory);
            Includes.Add(c => c.Sections);
            IncludesString.Add("Sections.Lectures");
            IncludesString.Add("Sections.Sessions");
            Includes.Add(c => c.Instructor);
            IncludesString.Add("Instructor.ApplicationUser");
        }
    }
}
