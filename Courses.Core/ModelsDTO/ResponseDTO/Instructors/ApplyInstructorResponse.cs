using Courses.Core.Models.Instructors;

namespace Courses.Core.ModelsDTO.ResponseDTO.Instructors
{
    public class ApplyInstructorResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public InstructorRequestStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}