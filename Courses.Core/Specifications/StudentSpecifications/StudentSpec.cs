using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.RequestDTO.Students;

namespace Courses.Core.Specifications.StudentSpecifications
{
    public class StudentSpec : BaseSpecifications<Student>
    {
        public StudentSpec(string id)
            :base(S => S.UserId == id)
        {
            Includes.Add(s => s.ApplicationUser);
        }

        public StudentSpec(DateTime fromDate, DateTime toDate)
            : base(x =>
                (x.CreatedAt >= fromDate)&&
                (x.CreatedAt <= toDate)
            )
        {
            IsTracking = false;
        }
    }
}
