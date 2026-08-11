using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.ResponseDTO.Profiles;
using Courses.Core.Services.Contract.ProfileServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Courses.Api.Controllers.Profile
{
    [Authorize(Roles = $"{Roles.Admin}, {Roles.Instructor}, {Roles.Student}")]
    public class ProfileController : BaseController
    {
        protected readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        #region Edit Profile
        [HttpPost("Edit/{userId}")] // POST: /api/Profile/Edit/{userId}
        public async Task<ActionResult<ApplicationServiceResult<UserProfileResponse>>> EditProfile([FromBody]EditProfileRequest req, string userId)
        {
            var result = await _profileService.EditProfileAsync(req, userId);
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
