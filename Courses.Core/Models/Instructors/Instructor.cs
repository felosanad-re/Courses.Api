using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Courses;

namespace Courses.Core.Models.Instructors
{
    /// <summary>
    /// Represents an instructor who creates and teaches courses.
    /// Inherits personal info (Name, Birthday, Age) from PersonalBase.
    /// </summary>
    public class Instructor : PersonalBase
    {

        // The instructor's area of expertise (e.g., "Web Development", "Data Science")
        public string Specialization { get; set; }

        // Navigation to the Identity user account (one-to-one with ApplicationUser)
        public ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }

        // All courses created by this instructor (one-to-many)
        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
