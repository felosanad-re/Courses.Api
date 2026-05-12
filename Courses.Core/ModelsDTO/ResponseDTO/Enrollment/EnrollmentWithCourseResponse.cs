using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.ModelsDTO.ResponseDTO.Enrollment
{
    public class EnrollmentWithCourseResponse
    {
        public EnrollStatus Status { get; set; }
        public int EnrollmentId { get; set; }
        public CourseDetailsToReturnDTO? Course { get; set; }
        public int? CourseId { get; set; }
        public string UserId { get; set; }
        public string? CheckOutURL { get; set; }
    }
}
