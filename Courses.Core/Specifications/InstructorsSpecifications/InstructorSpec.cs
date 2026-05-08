using Courses.Core.Models.Instructors;

namespace Courses.Core.Specifications.InstructorsSpecifications
{
    public class InstructorSpec : BaseSpecifications<Instructor>
    {
        public InstructorSpec(string id)
            :base(I => I.UserId == id)
        {

        }

        public InstructorSpec()
            : base()
        {
            Includes.Add(i => i.ApplicationUser);
            Includes.Add(i => i.Courses);
        }

        public InstructorSpec(int id)
            :base(I => I.Id == id)
        {
            Includes.Add(i => i.ApplicationUser);
            Includes.Add(i => i.Courses);
        }
    }
}
