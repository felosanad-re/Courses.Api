using Courses.Core.Models.Instructors;

namespace Courses.Core.Specifications.InstructorRequestSpecifications
{
    public class InstructorRequestSpec : BaseSpecifications<InstructorRequest>
    {

        public InstructorRequestSpec()
            :base()
        {
            Includes.Add(x => x.User);
        }

        public InstructorRequestSpec(string? userId)
            :base(x => 
            (x.UserId == userId) &&
            (x.Status == InstructorRequestStatus.Pending)
            )
        {
            Includes.Add(x => x.User);
        }
    }
}
