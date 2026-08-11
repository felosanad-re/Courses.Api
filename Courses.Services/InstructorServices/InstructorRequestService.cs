using AutoMapper;
using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.InstructorRequestSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Courses.Services.InstructorServices
{
    public class InstructorRequestService : IInstructorRequestService
    {
        private const string ErrorMessage = "User not found";
        #region Inject Services

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ILogger<InstructorRequestService> _logger;
        protected readonly IMapper _mapper;

        public InstructorRequestService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            ILogger<InstructorRequestService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _logger = logger;
            _mapper = mapper;
        }

        #endregion

        #region ApplyInstructorRequest

        public async Task<ApplicationServiceResult<ApplyInstructorResponse>> ApplyInstructorRequest(ApplyInstructorRequest req)
        {

            string? userId = null;
            string? userName = null;
            try
            {
                userId = _currentUserService.UserId;
                userName = _currentUserService.UserName;

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail(ErrorMessage);

                // Check if user is already an instructor
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains(Roles.Instructor))
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("You are already an instructor");

                // Check if there's already a pending request
                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();

                var existingRequest = await instructorRequestRepo.GetAsyncSpec(new InstructorRequestSpec(userId));
                if (existingRequest != null)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("You already have a pending request");

                if(req.ExperienceYears > 10)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("Your Experience Years have be more then 10 years");

                // Create new request
                var instructorRequest = new InstructorRequest
                {
                    UserId = userId,
                    Bio = req.Bio,
                    Specialty = req.Specialty,
                    ExperienceYears = req.ExperienceYears,
                    Status = InstructorRequestStatus.Pending,
                    CreatedBy = userName,
                    CreatedAt = DateTime.UtcNow
                };

                await instructorRequestRepo.AddAsync(instructorRequest);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<ApplyInstructorResponse>(instructorRequest);
                return ApplicationServiceResult<ApplyInstructorResponse>.Success(data, "Your request has been submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to apply request for user {userId}", userId);
                return ApplicationServiceResult<ApplyInstructorResponse>.Fail("There is an error in the database");
            }
        }

        #endregion
    }
}