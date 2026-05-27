using Courses.Core.Models.Enrollments;

namespace Courses.Core.Specifications.LectureSpecifications
{
    public class LectureWithInstructorSpec : BaseSpecifications<Lecture>
    {
        public LectureWithInstructorSpec(int lectureId, int? instructorId)
            : base(x =>
                (x.Id == lectureId) &&
                (x.Section.Course.InstructorId == instructorId)
            )
        {
            Includes.Add(x => x.Section);
        }

        public LectureWithInstructorSpec(IEnumerable<int> lectureIds, int? instructorId)
            : base(x =>
                (lectureIds.Contains(x.Id)) &&
                (x.Section.Course.InstructorId == instructorId)
            )
        {
            Includes.Add(x => x.Section);
        }
    }
}
