using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Account
{
    public class CheckAccountRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; }
    }
}
