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

        public CoursesCountWithSpec(int? instructorId, DateTime oneMonthAgo)
            :base(x => 
                (x.InstructorId == instructorId)&&
                (x.CreatedAt >= oneMonthAgo)
            )
        {
            
        }
        public CoursesCountWithSpec(int? instructorId)
            :base(x => 
                (x.InstructorId == instructorId)
            )
        {
            
        }

        public CoursesCountWithSpec()
            : base()
        {

        }

        public CoursesCountWithSpec(CourseStatus status)
            : base(x => x.Status == status)
        {
            
        }
    }
}
