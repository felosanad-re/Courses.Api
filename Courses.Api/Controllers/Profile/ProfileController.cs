using Courses.Api.ErrorHandler;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
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
        public async Task<ActionResult<ApplicationServiceResult<ApplicationUser>>> EditProfile(EditProfileRequest req)
        {
            var result = await _profileService.EditProfile(req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400)
            {
                StatusCode = 400,
                Message = [result.Message]
            });

            return Ok(result);
        }
        #endregion
    }
}
