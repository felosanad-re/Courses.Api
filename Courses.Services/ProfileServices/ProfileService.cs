using AutoMapper;
using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.ModelsDTO.ResponseDTO.Profiles;
using Courses.Core.Services.Contract.ProfileServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.InstructorsSpecifications;
using Courses.Core.Specifications.StudentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Courses.Services.ProfileServices
{
    public class ProfileService : IProfileService
    {
        private const string NotFoundMessage = "User Not Found";
        private const string LogError = "there is a problem in database";

        #region Inject Services
        protected readonly ICurrentUserService _currentUserService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<ProfileService> _logger;
        public ProfileService(UserManager<ApplicationUser> userManager, ILogger<ProfileService> logger, IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        #endregion

        public async Task<ApplicationServiceResult<UserProfileResponse>> GetUserProfileAsync()
        {
            const string ErrorMessage = "No User With this Id";
            const string SucceddedMessage = "User profile retrieved successfully";

            string? userId = null;

            try
            {
                userId = _currentUserService.UserId;
                if (string.IsNullOrWhiteSpace(userId))
                    return ApplicationServiceResult<UserProfileResponse>.Fail(NotFoundMessage);

                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                    return ApplicationServiceResult<UserProfileResponse>.Fail(ErrorMessage);
                // get user Roles
                var userRoles = await _userManager.GetRolesAsync(user);
                var data = _mapper.Map<UserProfileResponse>(user);
                data.UserRoles = userRoles.ToList();
                return ApplicationServiceResult<UserProfileResponse>.Success(data, SucceddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem when try to retrieve user {userId}", userId);
                return ApplicationServiceResult<UserProfileResponse>.Fail(LogError);
            }
        }

        #region Edit Profile
        public async Task<ApplicationServiceResult<UserProfileResponse>> EditProfileAsync(EditProfileRequest req, string userId)
        {
            const string SucceddedMessage = "User Edit Profile Succeeded";

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null) return ApplicationServiceResult<UserProfileResponse>.Fail(NotFoundMessage);

                _mapper.Map(req, user);
                var result = await _userManager.UpdateAsync(user); // Update Profile In Application User
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ApplicationServiceResult<UserProfileResponse>.Fail(errors);
                }

                var data = _mapper.Map<UserProfileResponse>(user);

                return ApplicationServiceResult<UserProfileResponse>.Success(data, SucceddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is problem when try to update profile for userId {userId}", userId);
                return ApplicationServiceResult<UserProfileResponse>.Fail(LogError);
            }
        }
        #endregion
    }
}
