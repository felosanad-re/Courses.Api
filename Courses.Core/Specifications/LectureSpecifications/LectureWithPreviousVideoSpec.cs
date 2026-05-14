using Courses.Core.Models.Enrollments;

namespace Courses.Core.Specifications.LectureSpecifications
{
    public class LectureWithPreviousVideoSpec : BaseSpecifications<Lecture>
    {
        public LectureWithPreviousVideoSpec(int sectionId, int currentVideoOrder)
            : base(x =>
                (x.SectionId == sectionId) &&
                (x.Order < currentVideoOrder)
            )
        {
            AddOrderByDesc(x => x.Order);
            Includes.Add(x => x.Section);
        }
    }
}
