using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO
{
    public class LoginRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
