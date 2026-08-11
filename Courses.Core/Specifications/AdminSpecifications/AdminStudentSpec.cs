using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO.RequestDTO.Students;

namespace Courses.Core.Specifications.AdminSpecifications
{
    public class AdminStudentSpec : BaseSpecifications<Student>
    {
        public AdminStudentSpec(StudentParams param)
            :base(x =>
                (string.IsNullOrWhiteSpace(param.Search) ||
                x.ApplicationUser.FullName.Contains(param.Search))
            )
        {
            IsTracking = false;
            Includes.Add(x => x.Enrollments);
            Includes.Add(x => x.ApplicationUser);
            AddPagination(param.PageSize * (param.PageIndex - 1), param.PageSize);
        }

        public AdminStudentSpec(int studentId)
            :base(x => x.Id == studentId)
        {
            IsTracking = false;
            Includes.Add(x => x.Enrollments);
            Includes.Add(x => x.ApplicationUser);
            IncludesString.Add("Enrollments.Course");
        }
    }
}
