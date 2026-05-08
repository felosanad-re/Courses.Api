using Courses.Core.Models.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CourseWithInstructorSpec : BaseSpecifications<Course>
    {
        public CourseWithInstructorSpec(IEnumerable<int> coursesIds)
            : base(c => coursesIds.Contains(c.Id))
        {
            Includes.Add(c => c.Instructor);
        }

        public CourseWithInstructorSpec(string userId)
            :base(c => c.Instructor.UserId == userId)
        {
            Includes.Add(c => c.Instructor);
        }

        public CourseWithInstructorSpec(int id, string userId)
            : base(c =>
                    (c.Id == id) && (c.Instructor.UserId == userId)
                  )
        {
            Includes.Add(c => c.Instructor);
        }

        public CourseWithInstructorSpec(IEnumerable<int> coursesIds, string userId)
            :base(c => 
                (coursesIds.Contains(c.Id))
                &&
                (c.Instructor.UserId == userId)
            )
        {
            Includes.Add(c => c.Instructor);
        }
    }
}
