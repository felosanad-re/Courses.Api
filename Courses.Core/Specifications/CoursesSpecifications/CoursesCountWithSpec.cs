using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CoursesCountWithSpec : BaseSpecifications<Course>
    {
        public CoursesCountWithSpec(CoursesParams @params)
            :base()
        {

        }
    }
}
