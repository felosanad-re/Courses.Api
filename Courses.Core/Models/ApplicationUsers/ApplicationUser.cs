using Microsoft.AspNetCore.Identity;

namespace Courses.Core.Models.ApplicationUsers
{
    /// <summary>
    /// Extended IdentityUser for authentication & authorization.
    /// This is the ASP.NET Core Identity user that both Student and Instructor
    /// link to via their UserId foreign key (one-to-one relationship).
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }


        // Soft delete flag — separate from Identity's LockoutEnd
        public bool IsDeleted { get; set; }
    }
}
