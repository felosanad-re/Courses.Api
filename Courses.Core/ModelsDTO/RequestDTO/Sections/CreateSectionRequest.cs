using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Sections
{
    public class CreateSectionRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
