using Courses.Core.ModelsDTO.ResponseDTO.Students;

namespace Courses.Core.ModelsDTO.ResponseDTO.Instructors
{
    public class StudentWithInstructorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseCount { get; set; }

        public DateTime FirstEnrollment { get; set; }
        public List<StudentCourseWithInstructor> Courses { get; set; } = new();
    }
}
