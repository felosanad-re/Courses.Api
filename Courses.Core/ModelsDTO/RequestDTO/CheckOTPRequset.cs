using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO
{
    public class CheckOTPRequest
    {
        [Required]
        public int OTP { get; set; }
    }
}
