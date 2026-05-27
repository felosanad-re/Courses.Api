using Courses.Core.Models.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CourseWithInstructorSpec : BaseSpecifications<Course>
    {
        public CourseWithInstructorSpec(IEnumerable<int> coursesIds)
            : base(c => coursesIds.Contains(c.Id))
        {
            Includes.Add(c => c.Instructor);
            Includes.Add(c => c.CourseType);
            IncludesString.Add("Sections.Lectures");
        }

        public CourseWithInstructorSpec(string userId)
            :base(c => c.Instructor.UserId == userId)
        {
            Includes.Add(c => c.Instructor);
            Includes.Add(c => c.CourseType);
        }

        public CourseWithInstructorSpec(int id, string userId)
            : base(c =>
                    (c.Id == id) && (c.Instructor.UserId == userId)
                  )
        {
            Includes.Add(c => c.Instructor);
            Includes.Add(c => c.CourseType);
            IncludesString.Add("Sections.Lectures");
        }

        public CourseWithInstructorSpec(IEnumerable<int> coursesIds, string userId)
            :base(c => 
                (coursesIds.Contains(c.Id))
                &&
                (c.Instructor.UserId == userId)
            )
        {
            Includes.Add(c => c.Instructor);
            Includes.Add(c => c.CourseType);
            IncludesString.Add("Sections.Lectures");
        }
    }
}
