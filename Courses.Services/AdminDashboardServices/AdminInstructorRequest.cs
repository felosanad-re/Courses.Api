using AutoMapper;
using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.InstructorRequestSpecifications;
using Courses.Core.UnitOfWork;
using Courses.Services.InstructorServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ApplicationServiceResult<Pagination<ApplyInstructorResponse>>> GetAllRequests([FromQuery]InstructorRequestParams param)
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

        #region Get Request Details
        public async Task<ApplicationServiceResult<ApplyInstructorDetailsResponse>> GetRequestDetails(int reqId)
        {
            string? userId = null;
            string? adminName = null;

            try
            {
                userId = _currentUserService.UserId;
                adminName = _currentUserService.UserName;
                var instructorRequestSpec = new BaseSpecifications<InstructorRequest>(x => x.Id == reqId);
                instructorRequestSpec.Includes.Add(x => x.User);
                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();

                var instructorRequest = await instructorRequestRepo.GetAsyncSpec(instructorRequestSpec);
                if (instructorRequest is null)
                    return ApplicationServiceResult<ApplyInstructorDetailsResponse>.Fail("There is No Instructor request with this id");

                var data = _mapper.Map<ApplyInstructorDetailsResponse>(instructorRequest);

                return ApplicationServiceResult<ApplyInstructorDetailsResponse>.Success(data, "you retrieve instructor request successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve data for request {reqId}, for admin name {adminName}", reqId, adminName);
                return ApplicationServiceResult<ApplyInstructorDetailsResponse>.Fail("There is an error in the database");
            }
        }
        #endregion

        #region ApproveRequest

        public async Task<ApplicationServiceResult<bool>> ApproveRequest(int reqId)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;
                var userName = _currentUserService.UserName;
                if (string.IsNullOrEmpty(currentUserId))
                    return ApplicationServiceResult<bool>.Fail("User not authenticated");

                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();
                var request = await instructorRequestRepo.GetAsync(reqId);
                if (request == null)
                    return ApplicationServiceResult<bool>.Fail("Request not found");

                if (request.Status != InstructorRequestStatus.Pending)
                    return ApplicationServiceResult<bool>.Fail("Request is not pending");

                // Update request status
                request.Status = InstructorRequestStatus.Approved;
                request.CreatedBy = userName;
                request.CreatedAt = DateTime.UtcNow;

                // Add Instructor role to user
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user != null)
                    await _userManager.AddToRoleAsync(user, Roles.Instructor);

                await _unitOfWork.CompleteAsync();

                return ApplicationServiceResult<bool>.Success(true, "Request approved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<bool>.Fail("There is an error in the database");
            }
        }

        #endregion

        #region RejectRequest

        public async Task<ApplicationServiceResult<bool>> RejectRequest(int reqId)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;
                var userName = _currentUserService.UserName;
                if (string.IsNullOrEmpty(currentUserId))
                    return ApplicationServiceResult<bool>.Fail("User not authenticated");

                var instructorRequestRepo = _unitOfWork.CreateRepository<InstructorRequest>();
                var request = await instructorRequestRepo.GetAsync(reqId);
                if (request == null)
                    return ApplicationServiceResult<bool>.Fail("Request not found");

                if (request.Status != InstructorRequestStatus.Pending)
                    return ApplicationServiceResult<bool>.Fail("Request is not pending");

                // Update request status
                request.Status = InstructorRequestStatus.Rejected;
                request.CreatedBy = userName;
                request.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                var user = await _userManager.FindByIdAsync(request.UserId);
                return ApplicationServiceResult<bool>.Success(true, "Request rejected successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<bool>.Fail("There is an error in the database");
            }
        }

        #endregion
    }
}
