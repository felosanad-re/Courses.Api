using Courses.Core.Models.Courses;
using Courses.Core.Models.LiveSessions;

namespace Courses.Core.Models.Enrollments
{
    /// <summary>
    /// A section/chapter within a Course. Each Section contains multiple Lectures.
    /// Belongs to one Course (many-to-one).
    /// </summary>
    public class Section : BaseModel
    {
        public string Title { get; set; }

        // To get next and previous
        public int Order { get; set; }

        // The course this section belongs to (many-to-one)
        public Course Course { get; set; }
        public int CourseId { get; set; }

        // All lectures within this section (one-to-many)
        public ICollection<Lecture> Lectures { get; set; } = new HashSet<Lecture>();

        // All Live Session (one-to-many)
        public ICollection<LiveSession> Sessions { get; set; } = new HashSet<LiveSession>();
    }
}
