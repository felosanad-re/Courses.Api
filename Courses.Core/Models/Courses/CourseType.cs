namespace Courses.Core.Models.Courses
{
    /// <summary>
    /// Category/type for courses (e.g., "Programming", "Design", "Business").
    /// A Course belongs to one CourseType; a CourseType has many Courses.
    /// </summary>
    public class CourseType
    {
        public int Id { get; set; }
        public string Name { get; set; }


        // All courses in this category (one-to-many)
        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
