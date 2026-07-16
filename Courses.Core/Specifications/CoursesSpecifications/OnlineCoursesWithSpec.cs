using Courses.Core.Models.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class OnlineCoursesWithSpec :BaseSpecifications<Course>
    {
        public OnlineCoursesWithSpec(int courseId)
            : base(x => x.Id == courseId && x.Type == CourseType.OnlineCourse)
        {
            Includes.Add(c => c.CourseCategory);
            Includes.Add(c => c.Sections);
            IncludesString.Add("Sections.Lectures");
            IncludesString.Add("Sections.Sessions");
            Includes.Add(c => c.Instructor);
        }
    }
}
