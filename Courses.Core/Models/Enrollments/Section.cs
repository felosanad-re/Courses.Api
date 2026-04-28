using Courses.Core.Models.Courses;

namespace Courses.Core.Models.Enrollments
{
    /// <summary>
    /// A section/chapter within a Course. Each Section contains multiple Lectures.
    /// Belongs to one Course (many-to-one).
    /// </summary>
    public class Section
    {
        public int Id { get; set; }
        public string Title { get; set; }


        // The course this section belongs to (many-to-one)
        public Course Course { get; set; }
        public int CourseId { get; set; }

        // All lectures within this section (one-to-many)
        public ICollection<Lecture> Lectures { get; set; } = new HashSet<Lecture>();
    }
}
