using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Sections
{
    public class UpdateSectionRequest
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
