using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Sections
{
    public class UpdateSectionRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Order { get; set; }
    }
}
