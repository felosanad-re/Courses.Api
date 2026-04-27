using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO
{
    public class CheckOTPRequest
    {
        [Required]
        public int OTP { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
