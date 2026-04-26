using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO
{
    public class CheckAccountRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; }
    }
}
