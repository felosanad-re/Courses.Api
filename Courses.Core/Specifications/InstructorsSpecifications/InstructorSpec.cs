using Courses.Core.Models.Instructors;

namespace Courses.Core.Specifications.InstructorsSpecifications
{
    public class InstructorSpec : BaseSpecifications<Instructor>
    {
        public InstructorSpec(string id)
            :base(I => I.UserId == id)
        {
            
        }
    }
}
