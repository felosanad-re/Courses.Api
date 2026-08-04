using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CoursesWithAdminCountSpec : BaseSpecifications<Course>
    {
        public CoursesWithAdminCountSpec(CoursesParams @params, CourseType? courseType, CourseStatus? status)
            : base(x =>
                (!courseType.HasValue || x.Type == courseType) &&
                (!status.HasValue || x.Status == status)
            )
        {
            
        }
    }
}
