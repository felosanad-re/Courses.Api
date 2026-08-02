using Courses.Core.Models.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CoursesWithoutIncludesSpec : BaseSpecifications<Course>
    {
        public CoursesWithoutIncludesSpec(int courseId)
            :base(x => x.Id == courseId)
        {
            
        }
    }
}
