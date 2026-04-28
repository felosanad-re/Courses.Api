using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;

namespace Courses.Core.Models.Courses
{
    /// <summary>
    /// Represents a course on the platform. Created by an Instructor,
    /// categorized by CourseType, and contains Sections with Lectures.
    /// Students enroll via the Enrollment entity.
    /// </summary>
    public class Course : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }


        // The instructor who created this course (many-to-one)
        public Instructor Instructor { get; set; }
        public int InstructorId { get; set; }


        // The category/type of this course (many-to-one)
        public CourseType CourseType { get; set; }
        public int CourseTypeId { get; set; }


        // Students enrolled in this course (one-to-many via Enrollment join)
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();

        // Sections containing lectures for this course (one-to-many)
        public ICollection<Section> Sections { get; set; } = new HashSet<Section>();
    }
}
