using AutoMapper;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.AdminSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminManagementInstructors : IAdminManagementInstructors
    {
        #region Service
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ILogger<AdminManagementInstructors> _logger;
        protected readonly IMapper _mapper;

        public AdminManagementInstructors(IUnitOfWork unitOfWork, ILogger<AdminManagementInstructors> logger, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        #endregion

        #region Get All Instructors Async
        public async Task<ApplicationServiceResult<Pagination<AdminInstructorResponse>>> GetAllInstructorsAsync(InstructorParams param)
        {
            const string SucceededMessage = "You get all Instructors Succeeded.";
            const string WarningMessage = "There is no Instructors yet.";
            const string LoggerMessage = "Failed to retrieve Instructors.";
            string? userId = _currentUserService.UserId;

            try
            {
                var instructorSpec = new AdminWithInstructorSpec(param);
                var instructorCountSpec = new AdminWithInstructorCountSpec(param);
                var instructorRepo = _unitOfWork.CreateRepository<Instructor>();

                var totalInstructors = await instructorRepo.GetCountAsyncSpec(instructorCountSpec);
                if (totalInstructors <= 0)
                    return ApplicationServiceResult<Pagination<AdminInstructorResponse>>.Success(new Pagination<AdminInstructorResponse>(param.PageIndex, param.PageSize, 0, []), WarningMessage);

                var instructors = await instructorRepo.GetAllAsyncSpec(instructorSpec);

                var data = _mapper.Map<IReadOnlyList<AdminInstructorResponse>>(instructors);
                var pagnation = new Pagination<AdminInstructorResponse>(param.PageIndex, param.PageSize, totalInstructors, data);

                return ApplicationServiceResult<Pagination<AdminInstructorResponse>>.Success(pagnation, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem when try to retrieve instructors for userId {userId}", userId);
                return ApplicationServiceResult<Pagination<AdminInstructorResponse>>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region Get Instructor Details Async
        public async Task<ApplicationServiceResult<AdminInstructorDetailsResponse>> GetInstructorDetailsAsync(int instructorId)
        {
            const string SucceededMessage = "You get Instructor Details Succeeded.";
            const string WarningMessage = "There is no Instructor with this id.";
            const string LoggerMessage = "Failed to retrieve Instructor.";
            string? userId = _currentUserService.UserId;

            try
            {
                var instructorSpec = new AdminWithInstructorSpec(instructorId);
                var instructorRepo = _unitOfWork.CreateRepository<Instructor>();

                var instructor = await instructorRepo.GetAsyncSpec(instructorSpec);
                if (instructor is null)
                    return ApplicationServiceResult<AdminInstructorDetailsResponse>.Fail(WarningMessage);

                var data = _mapper.Map<AdminInstructorDetailsResponse>(instructor);

                return ApplicationServiceResult<AdminInstructorDetailsResponse>.Success(data, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve instructorId {instructorId} for userId {userId}", instructorId, userId);
                return ApplicationServiceResult<AdminInstructorDetailsResponse>.Fail(LoggerMessage);
            }
        }
        #endregion
    }
}