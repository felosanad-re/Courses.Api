using Courses.Core.Models.ApplicationUsers;

namespace Courses.Core.Services.Contract
{
    public interface ICreateToken
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
