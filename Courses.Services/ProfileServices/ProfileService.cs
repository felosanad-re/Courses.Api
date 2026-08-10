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
        public async Task<ApplicationServiceResult<ApplicationUser>> EditProfileAsync(EditProfileRequest req)
        {
            const string ErrorMessage = "User Not Updated";
            const string SucceddedMessage = "User Edit Profile Succeeded";

            try
            {
                var user = await _userManager.FindByIdAsync(req.Id);
                if (user is null) return ApplicationServiceResult<ApplicationUser>.Fail(NotFoundMessage);

                var roles = await _userManager.GetRolesAsync(user);

                _mapper.Map(req, user);
                var result = await _userManager.UpdateAsync(user); // Update Profile In Application User

                if(!result.Succeeded) return ApplicationServiceResult<ApplicationUser>.Fail(ErrorMessage);

                bool isUpdated = false;

                // Update Student
                if(roles.Contains(Roles.Student))
                    isUpdated |= await UpdateStudent(req);

                // Update Instructor
                if (roles.Contains(Roles.Instructor))
                    isUpdated |= await UpdateInstructor(req);

                // Save Changes
                if (isUpdated)
                    await _unitOfWork.CompleteAsync();

                return ApplicationServiceResult<ApplicationUser>.Success(user, SucceddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<ApplicationUser>.Fail(LogError);
            }
        }
        #endregion

        #region Helper Method
        private async Task<bool> UpdateInstructor(EditProfileRequest req)
        {
            var spec = new InstructorSpec(req.Id);
            var instructorRepo = _unitOfWork.CreateRepository<Instructor>();
            var instructor = await instructorRepo.GetAsyncSpec(spec);
            if (instructor == null)
                return false;

            return true;
        }

        private async Task<bool> UpdateStudent(EditProfileRequest req)
        {
            var spec = new StudentSpec(req.Id);
            var studentRepo = _unitOfWork.CreateRepository<Student>();
            var student = await studentRepo.GetAsyncSpec(spec);
            if (student == null)
                return false;

            _mapper.Map(req, student);
            return true;
        }
        #endregion
    }
}
