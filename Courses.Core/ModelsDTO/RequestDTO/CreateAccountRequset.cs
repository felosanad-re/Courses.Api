using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO
{
    public class CreateAccountRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$",
                ErrorMessage = "Password must contain letters and numbers")]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "The Confirm Password Not Match WIth Password")]
        public string ConfirmPassword { get; set; }
    }
}
