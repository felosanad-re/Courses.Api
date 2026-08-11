using AutoMapper;
using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.InstructorRequestSpecifications;
using Courses.Core.UnitOfWork;
using Courses.Services.InstructorServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminInstructorRequest : IAdminInstructorRequest
    {

        #region Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ILogger<InstructorRequestService> _logger;
        protected readonly IMapper _mapper;

        public AdminInstructorRequest(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService, ILogger<InstructorRequestService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        #region GetAllRequests

        public async Task<ApplicationServiceResult<Pagination<ApplyInstructorResponse>>> GetAllRequests(InstructorRequestParams param)
        {
            try
            {
                param.Search = param.Search?.ToLower().Trim();
                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();
                var instructorSpec = new InstructorRequestSpec(param);
                var instructorCountSpec = new InstructorCountRequest(param);

                var totalCountRequests = await instructorRequestRepo.GetCountAsyncSpec(instructorCountSpec);

                if (totalCountRequests == 0)
                    return ApplicationServiceResult<Pagination<ApplyInstructorResponse>>.Success(new Pagination<ApplyInstructorResponse>(param.PageIndex, param.PageSize, 0, []), "This all instructors requests");
                var requests = await instructorRequestRepo.GetAllAsyncSpec(instructorSpec);

                var data = _mapper.Map<IReadOnlyList<ApplyInstructorResponse>>(requests);
                var pagnator = new Pagination<ApplyInstructorResponse>(
                        param.PageIndex,
                        param.PageSize,
                        totalCountRequests,
                        data
                    );
                return ApplicationServiceResult<Pagination<ApplyInstructorResponse>>.Success(pagnator, "This all instructors requests");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<Pagination<ApplyInstructorResponse>>.Fail("There is an error in the database");
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
                    await _userManager.AddToRoleAsync(user, Roles.Instructor);

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

        #region Private Helper Methods

        private static ApplyInstructorResponse MapToResponse(InstructorRequest request, ApplicationUser? user)
        {
            return new ApplyInstructorResponse
            {
                Id = request.Id,
                UserId = request.UserId,
                FullName = user?.FullName ?? string.Empty,
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
