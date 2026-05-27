using Courses.Core.Models.Enrollments;

namespace Courses.Core.Specifications.SectionsSpecifications
{
    public class SectionWithCourseSpec : BaseSpecifications<Section>
    {
        public SectionWithCourseSpec(int sectionId, int? instructorId)
            :base(x => 
                (x.Id == sectionId) &&
                (x.Course.InstructorId == instructorId)
            )
        {
            Includes.Add(x => x.Course);
            Includes.Add(x => x.Lectures);
        }

        public SectionWithCourseSpec(IEnumerable<int> ids, int? instructorId)
            :base(x =>
                    (ids.Contains(x.Id)) &&
                    (x.Course.InstructorId == instructorId)
            )
        {
            Includes.Add(x => x.Course);
            Includes.Add(x => x.Lectures);
        }
    }
}
