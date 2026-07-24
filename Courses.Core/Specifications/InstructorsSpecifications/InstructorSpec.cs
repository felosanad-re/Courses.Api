using Courses.Core.Models.Instructors;

namespace Courses.Core.Specifications.InstructorsSpecifications
{
    public class InstructorSpec : BaseSpecifications<Instructor>
    {
        public InstructorSpec(int instructorId)
            :base(I => I.Id == instructorId)
        {
            Includes.Add(i => i.ApplicationUser);
            Includes.Add(i => i.Courses);
        }

        public InstructorSpec()
            : base()
        {
            Includes.Add(i => i.ApplicationUser);
            Includes.Add(i => i.Courses);
        }

        public InstructorSpec(InstructorStatus status)
            : base(x => x.Status == status)
        {

        }

        public InstructorSpec(string userId)
            :base(x => x.UserId == userId)
        {
            
        }
    }
}
