using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Profile;

namespace Courses.Core.Services.Contract.ProfileServices
{
    public interface IProfileService
    {
        Task<ApplicationServiceResult<ApplicationUser>> EditProfile(EditProfileRequest req);
    }
}
