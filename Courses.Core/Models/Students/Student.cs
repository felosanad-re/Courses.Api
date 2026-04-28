using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Enrollments;

namespace Courses.Core.Models.Students
{
    /// <summary>
    /// Represents a student on the platform. Inherits personal info (Name, Birthday, Age)
    /// from PersonalBase. Links to an Identity user account via UserId.
    /// </summary>
    public class Student : PersonalBase
    {
        // Navigation to the Identity user account (one-to-one with ApplicationUser)
        public ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }


        // All courses this student is enrolled in (one-to-many via Enrollment)
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
    }
}
