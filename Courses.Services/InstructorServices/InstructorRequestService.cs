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
        #region Inject Services

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ILogger<InstructorRequestService> _logger;

        public InstructorRequestService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            ILogger<InstructorRequestService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        #endregion

        #region ApplyInstructorRequest

        public async Task<ApplicationServiceResult<ApplyInstructorResponse>> ApplyInstructorRequest(ApplyInstructorRequest req)
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(userId))
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("User not authenticated");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("User not found");

                // Check if user is already an instructor
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains(Roles.Instructor))
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("You are already an instructor");

                // Check if there's already a pending request
                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();
                // x => x.UserId == userId && x.Status == InstructorRequestStatus.Pending
                var existingRequest = await instructorRequestRepo.GetAsyncSpec(new InstructorRequestSpec(userId));
                if (existingRequest != null)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("You already have a pending request");

                // Create new request
                var instructorRequest = new InstructorRequest
                {
                    UserId = userId,
                    Bio = req.Bio,
                    Specialty = req.Specialty,
                    ExperienceYears = req.ExperienceYears,
                    Status = InstructorRequestStatus.Pending,
                    CreatedBy = userId
                };

                await instructorRequestRepo.AddAsync(instructorRequest);
                await _unitOfWork.CompleteAsync();

                var response = MapToResponse(instructorRequest, user);
                return ApplicationServiceResult<ApplyInstructorResponse>.Success(response, "Your request has been submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<ApplyInstructorResponse>.Fail("There is an error in the database");
            }
        }

        #endregion

        #region ApproveRequest

        public async Task<ApplicationServiceResult<ApplyInstructorResponse>> GetApproveRequest(int reqId)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId))
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("User not authenticated");

                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();
                var request = await instructorRequestRepo.GetAsync(reqId);
                if (request == null)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("Request not found");

                if (request.Status != InstructorRequestStatus.Pending)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("Request is not pending");

                // Update request status
                request.Status = InstructorRequestStatus.Approved;
                request.CreatedBy = currentUserId;
                request.CreatedAt = DateTime.UtcNow;

                // Add Instructor role to user
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Instructor);
                }

                await _unitOfWork.CompleteAsync();

                var response = MapToResponse(request, user!);
                return ApplicationServiceResult<ApplyInstructorResponse>.Success(response, "Request approved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<ApplyInstructorResponse>.Fail("There is an error in the database");
            }
        }

        #endregion

        #region RejectRequest

        public async Task<ApplicationServiceResult<ApplyInstructorResponse>> GetRejectRequest(int reqId)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId))
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("User not authenticated");

                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();
                var request = await instructorRequestRepo.GetAsync(reqId);
                if (request == null)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("Request not found");

                if (request.Status != InstructorRequestStatus.Pending)
                    return ApplicationServiceResult<ApplyInstructorResponse>.Fail("Request is not pending");

                // Update request status
                request.Status = InstructorRequestStatus.Rejected;
                request.CreatedBy = currentUserId;
                request.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                var user = await _userManager.FindByIdAsync(request.UserId);
                var response = MapToResponse(request, user!);
                return ApplicationServiceResult<ApplyInstructorResponse>.Success(response, "Request rejected successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<ApplyInstructorResponse>.Fail("There is an error in the database");
            }
        }

        #endregion

        #region GetAllRequests

        public async Task<ApplicationServiceResult<IReadOnlyList<ApplyInstructorResponse>>> GetAllRequests()
        {
            try
            {
                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();
                var requests = await instructorRequestRepo.GetAllAsyncSpec(new InstructorRequestSpec());

                var responses = new List<ApplyInstructorResponse>();
                foreach (var request in requests)
                {
                    var user = await _userManager.FindByIdAsync(request.UserId);
                    responses.Add(MapToResponse(request, user!));
                }

                return ApplicationServiceResult<IReadOnlyList<ApplyInstructorResponse>>.Success(responses, "This all instructors requests");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<IReadOnlyList<ApplyInstructorResponse>>.Fail("There is an error in the database");
            }
        }

        #endregion

        #region Private Helper Methods

        private static ApplyInstructorResponse MapToResponse(InstructorRequest request, ApplicationUser? user)
        {
            return new ApplyInstructorResponse
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = user?.UserName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Bio = request.Bio,
                Specialty = request.Specialty,
                ExperienceYears = request.ExperienceYears,
                Status = request.Status,
                RejectionReason = request.RejectionReason,
                CreatedAt = request.CreatedAt
            };
        }

        #endregion
    }
}