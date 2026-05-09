using Courses.Core.Models.ApplicationUsers;

namespace Courses.Core.Models.Instructors
{
    public class InstructorRequest : BaseModel
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // NFP

        public string Bio { get; set; }

        public string Specialty { get; set; }

        public int ExperienceYears { get; set; }

        public InstructorRequestStatus Status { get; set; }

        public string? RejectionReason { get; set; }
    }
}
