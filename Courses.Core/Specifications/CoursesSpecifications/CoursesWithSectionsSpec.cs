using Courses.Core.Models.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CoursesWithSectionsSpec : BaseSpecifications<Course>
    {
        public CoursesWithSectionsSpec(int courseId)
            : base(x => x.Id == courseId)
        {
            Includes.Add(x => x.Sections);
        }
    }
}
