using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.RequestDTO.Students;

namespace Courses.Core.Specifications.AdminSpecifications
{
    public class AdminStudentCountSpec : BaseSpecifications<Student>
    {
        public AdminStudentCountSpec(StudentParams param)
            :base()
        {
            IsTracking = false;
        }
    }
}
