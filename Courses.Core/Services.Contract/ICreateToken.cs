using Courses.Core.Models.ApplicationUsers;
using Microsoft.AspNetCore.Identity;

namespace Courses.Core.Services.Contract
{
    public interface ICreateToken
    {
        Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager);

    }
}
