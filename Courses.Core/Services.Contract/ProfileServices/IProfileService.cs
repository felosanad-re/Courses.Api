using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.ResponseDTO.Profiles;

namespace Courses.Core.Services.Contract.ProfileServices
{
    public interface IProfileService
    {
        Task<ApplicationServiceResult<UserProfileResponse>> GetUserProfileAsync();
        Task<ApplicationServiceResult<UserProfileResponse>> EditProfileAsync(EditProfileRequest req, string userId);
    }
}
