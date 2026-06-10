using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Courses;

namespace Courses.Core.Specifications.InstructorsSpecifications
{
    public class InstructorWithMyCoursesSpec: BaseSpecifications<Course>
    {
        public InstructorWithMyCoursesSpec(CoursesParams param, int? instructorId, bool isPagination = false)
            :base(x =>
                (x.InstructorId == instructorId) &&
                (string.IsNullOrEmpty(param.Search) || x.Name.ToLower().Contains(param.Search.Trim().ToLower()))
            )
        {
            if (isPagination)
            {
                Includes.Add(x => x.Enrollments);
                AddPagination(param.PageSize * (param.PageIndex - 1), param.PageSize);
            }
        }
    }
}
