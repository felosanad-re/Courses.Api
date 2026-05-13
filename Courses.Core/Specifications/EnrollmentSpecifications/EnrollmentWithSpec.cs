using Courses.Core.Models.Enrollments;

namespace Courses.Core.Specifications.EnrollmentSpecifications
{
    public class EnrollmentWithSpec : BaseSpecifications<Enrollment>
    {
        public EnrollmentWithSpec(int studentId, int courseId)
            :base(x => 
                (x.StudentId == studentId) &&
                (x.CourseId == courseId)
            )
        {
            Includes.Add(x => x.Student);
            Includes.Add(x => x.Course);
        }
    }
}
