using Courses.Core.Models.Students;

namespace Courses.Core.Specifications.StudentSpecifications
{
    public class StudentSpec : BaseSpecifications<Student>
    {
        public StudentSpec(string id)
            :base(S => S.UserId == id)
        {
            
        }
    }
}
