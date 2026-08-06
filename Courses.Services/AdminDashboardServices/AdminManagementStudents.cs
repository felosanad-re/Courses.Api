using AutoMapper;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Students;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.AdminSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminManagementStudents : IAdminManagementStudents
    {
        #region Services
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<AdminManagementStudents> _logger;
        protected readonly IMapper _mapper;

        public AdminManagementStudents(IUnitOfWork unitOfWork, ILogger<AdminManagementStudents> logger, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        #endregion

        #region Get Students Async
        public async Task<ApplicationServiceResult<Pagination<AdminWithStudentResponse>>> GetStudentsAsync(StudentParams param)
        {
            const string SucceededMessage = "You get all Students Succeeded.";
            const string WarningMessage = "There is no Students yet.";
            const string LoggerMessage = "Failed to retrieve Students.";
            string? userId = _currentUserService.UserId;

            try
            {
                param.Search = param.Search?.Trim().ToLower();
                var studentSpec = new AdminStudentSpec(param);
                var studentCountSpec = new AdminStudentCountSpec(param);
                var studentRepo = _unitOfWork.CreateRepository<Student>();

                var studentsCount = await studentRepo.GetCountAsyncSpec(studentCountSpec);
                if (studentsCount <= 0)
                    return ApplicationServiceResult<Pagination<AdminWithStudentResponse>>.Success(new Pagination<AdminWithStudentResponse>(param.PageIndex, param.PageSize, 0, new List<AdminWithStudentResponse>()), WarningMessage);

                var students = await studentRepo.GetAllAsyncSpec(studentSpec);

                var instructorUserIds = await _unitOfWork.CreateRepository<Instructor>()
                    .GetQuerySpec(new BaseSpecifications<Instructor>())
                    .Select(x => x.UserId)
                    .ToHashSetAsync();

                var data = _mapper.Map<IReadOnlyList<AdminWithStudentResponse>>(students);

                // To Know if user has instructor roles
                foreach (var user in data)
                {
                    user.IsInstructor = instructorUserIds.Contains(user.UserId);
                }

                var pagination = new Pagination<AdminWithStudentResponse>(
                        param.PageIndex,
                        param.PageSize,
                        studentsCount,
                        data
                    );

                return ApplicationServiceResult<Pagination<AdminWithStudentResponse>>.Success(pagination, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem when try to retrieve all students for userId {userId}", userId);

                return ApplicationServiceResult<Pagination<AdminWithStudentResponse>>.Success(new Pagination<AdminWithStudentResponse>(param.PageIndex, param.PageSize, 0, new List<AdminWithStudentResponse>()), LoggerMessage);
            }
        }
        #endregion

        #region Get Student Details Async
        public async Task<ApplicationServiceResult<AdminWithStudentDetailsResponse>> GetStudentDetailsAsync(int studentId)
        {
            const string SucceededMessage = "You get Student Details Succeeded.";
            const string WarningMessage = "There is no Student with this id.";
            const string LoggerMessage = "Failed to retrieve Students.";
            string? userId = _currentUserService.UserId;

            try
            {
                var studentSpec = new AdminStudentSpec(studentId);
                var studentRepo = _unitOfWork.CreateRepository<Student>();

                var student = await studentRepo.GetAsyncSpec(studentSpec);
                if (student is null)
                    return ApplicationServiceResult<AdminWithStudentDetailsResponse>.Fail(WarningMessage);

                var data = _mapper.Map<AdminWithStudentDetailsResponse>(student);
                data.NumberOfEnrollments = student.Enrollments.Count;
                return ApplicationServiceResult<AdminWithStudentDetailsResponse>.Success(data, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve student details {studentId}, for Admin {userId}", studentId, userId);
                return ApplicationServiceResult<AdminWithStudentDetailsResponse>.Fail(LoggerMessage);
            }
        }
        #endregion
    }
}
