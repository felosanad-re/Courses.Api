using AutoMapper;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.InstructorServices
{
    public class CurrentInstructorServices : ICurrentInstructorServices
    {
        #region DI Services
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<CurrentInstructorServices> _logger;
        protected readonly IMapper _mapper;
        public CurrentInstructorServices(ICurrentUserService currentUserService, ILogger<CurrentInstructorServices> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        #endregion

        public async Task<ApplicationServiceResult<InstructorWithApplicationUserResponse>> CurrentInstructor()
        {
            var userId = _currentUserService.UserId;
            if (userId == null) return ApplicationServiceResult<InstructorWithApplicationUserResponse>.Fail("there is no user with this id");
            try
            {
                var spec = new BaseSpecifications<Instructor>(x => x.UserId == userId);

                var instructor = await _unitOfWork.CreateRepository<Instructor>().GetAsyncSpec(spec);
                if (instructor is null) return ApplicationServiceResult<InstructorWithApplicationUserResponse>.Fail("Current user is not an instructor");
                var data = _mapper.Map<InstructorWithApplicationUserResponse>(instructor);

                return ApplicationServiceResult<InstructorWithApplicationUserResponse>.Success(data, "You retrieve instructor Id Succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve instructor by UserId : {userId}", userId ?? "NULL");
                return ApplicationServiceResult<InstructorWithApplicationUserResponse>.Fail("there is a problem in database");
            }
        }
    }
}
