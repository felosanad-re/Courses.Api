using Courses.Api.ErrorHandler;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.ResponseDTO.Profiles;
using Courses.Core.Services.Contract.ProfileServices;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Profile
{

    public class ProfileController : BaseController
    {
        protected readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        #region Edit Profile
        [HttpPost("EditProfile")] // POST: /api/Profile/EditProfile
        public async Task<ActionResult<ApplicationServiceResult<UserProfileResponse>>> EditProfile(EditProfileRequest req)
        {
            var result = await _profileService.EditProfileAsync(req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400)
            {
                StatusCode = 400,
                Message = [result.Message]
            });

            return Ok(result);
        }
        #endregion

        [HttpGet] // GET: /api/profile
        public async Task<ActionResult<ApplicationServiceResult<UserProfileResponse>>> GetProfile()
        {
            var res = await _profileService.GetUserProfileAsync();
            if (!res.Succeed) return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
    }
}
