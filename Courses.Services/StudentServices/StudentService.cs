using AutoMapper;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Students;
using Courses.Core.Services.Contract.StudentServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.StudentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.StudentServices
{
    public class StudentService : IStudentService
    {
        #region DI Service
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<StudentService> _logger;
        protected readonly IMapper _mapper;

        public StudentService(ICurrentUserService currentUserService, ILogger<StudentService> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Get Student With Application User
        public async Task<ApplicationServiceResult<StudentWithApplicationUserToReturnDTO>> GetStudentWithApplicationUser()
        {
            var userNotFoundError = "User Not Found With this id";
            var errorMessage = "there is no student with this id";
            var succeededMessage = "Revived Student Succeeded";
            var loggerError = "there is a problem in database";

            var userId = _currentUserService.UserId;
            if (userId == null) return ApplicationServiceResult<StudentWithApplicationUserToReturnDTO>.Fail(userNotFoundError);
            try
            {

                var student = await _unitOfWork.CreateRepository<Student>().GetAsyncSpec(new StudentSpec(userId));

                if (student is null) return ApplicationServiceResult<StudentWithApplicationUserToReturnDTO>.Fail(errorMessage);

                var data = _mapper.Map<StudentWithApplicationUserToReturnDTO>(student);

                return ApplicationServiceResult<StudentWithApplicationUserToReturnDTO>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve student by UserId : {userId}", userId ?? "NULL");
                return ApplicationServiceResult<StudentWithApplicationUserToReturnDTO>.Fail(loggerError);
            }
        }
        #endregion
    }
}
