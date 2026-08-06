using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;

namespace Courses.Core.Specifications.AdminSpecifications
{
    public class AdminWithInstructorCountSpec : BaseSpecifications<Instructor>
    {
        public AdminWithInstructorCountSpec(InstructorParams param)
            :base()
        {
            
        }
    }
}
