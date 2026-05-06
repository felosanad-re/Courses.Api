using AutoMapper;
using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Profile;
using Courses.Core.Services.Contract.ProfileServices;
using Courses.Core.Specifications.InstructorsSpecifications;
using Courses.Core.Specifications.StudentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Courses.Services.ProfileServices
{
    public class ProfileService : IProfileService
    {
        #region Inject Services
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<ProfileService> _logger;
        public ProfileService(UserManager<ApplicationUser> userManager, ILogger<ProfileService> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Edit Profile
        public async Task<ApplicationServiceResult<ApplicationUser>> EditProfile(EditProfileRequest req)
        {
            var notFoundMessage = "User Not Found";
            var errorMessage = "User Not Updated";
            var succeddedMessage = "User Edit Profile Succeeded";
            var logError = "there is a problem in database see log file to more information";

            try
            {
                var user = await _userManager.FindByIdAsync(req.Id);
                if (user is null) return ApplicationServiceResult<ApplicationUser>.Fail(notFoundMessage);

                var roles = await _userManager.GetRolesAsync(user);

                _mapper.Map(req, user);
                var result = await _userManager.UpdateAsync(user); // Update Profile In Application User

                if(!result.Succeeded) return ApplicationServiceResult<ApplicationUser>.Fail(errorMessage);

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

                return ApplicationServiceResult<ApplicationUser>.Success(user, succeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<ApplicationUser>.Fail(logError);
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

            _mapper.Map(req, instructor);
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
