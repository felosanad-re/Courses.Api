using Courses.Core.Models.Enrollments;

namespace Courses.Core.Specifications.SectionsSpecifications
{
    public class SectionWithSpec : BaseSpecifications<Section>
    {
        public SectionWithSpec(int courseId)
            : base(x => x.CourseId == courseId)
        {
            Includes.Add(x => x.Lectures);
        }

        public SectionWithSpec(int courseId, int instructorId)
            :base(x =>
                (x.CourseId == courseId)&&
                (x.Course.InstructorId == instructorId)
            )
        {

        }
    }
}
