using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.LiveSessions
{
    public class LiveSessionRequest : IValidatableObject
    {
        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        [Range(1, 1440)]
        public int Duration { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SectionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ScheduledAt <= DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Scheduled date must be in the future.",
                    new[] { nameof(ScheduledAt) }
                );
            }
        }
    }
}
