using Courses.Core.Models.Courses;

namespace Courses.Core.ModelsDTO.RequestDTO.Courses
{
    public class UpdateCourseStatusRequest
    {
        public CourseStatus Status { get; set; }
    }
}
