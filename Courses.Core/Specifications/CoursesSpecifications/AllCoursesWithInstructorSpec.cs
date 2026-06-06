using Courses.Core.Models.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class AllCoursesWithInstructorSpec : BaseSpecifications<Course>
    {
        public AllCoursesWithInstructorSpec(int? instructorId)
            :base(x => x.InstructorId == instructorId)
        {

        }
    }
}
