using Courses.Core.Models.Enrollments;

namespace Courses.Core.Specifications.LectureSpecifications
{
    public class LectureWithNextVideoSpec : BaseSpecifications<Lecture>
    {
        public LectureWithNextVideoSpec(int sectionId, int currentVideoOrder)
            : base(x =>
                (x.SectionId == sectionId) &&
                (x.Order > currentVideoOrder)
            )
        {
            AddOrderBy(x => x.Order);
            Includes.Add(x => x.Section);
        }
    }
}
