using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Sections
{
    public class CreateSectionRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Order { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }
    }
}
