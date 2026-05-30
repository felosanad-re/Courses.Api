using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.InstructorsSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.InstructorServices
{
    public class InstructorService : IInstructorService
    {
        #region Inject Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<InstructorService> _logger;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IMapper _mapper;

        public InstructorService(IUnitOfWork unitOfWork, ILogger<InstructorService> logger, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        #endregion

        #region Get All Instructors Async
        public async Task<ApplicationServiceResult<IReadOnlyList<InstructorResponse>>> GetAllInstructorsAsync()
        {
            var errorMessage = "There is no Instructors yet";
            var succeededMessage = "this all Instructors";
            var loggerError = "There is a problem in database";

            try
            {
                var spec = new InstructorSpec();
                var instructors = await _unitOfWork.CreateRepository<Instructor>().GetAllAsyncSpec(spec);
                if (!instructors.Any()) return ApplicationServiceResult<IReadOnlyList<InstructorResponse>>.Fail(errorMessage);

                var data = _mapper.Map<IReadOnlyList<InstructorResponse>>(instructors);

                return ApplicationServiceResult<IReadOnlyList<InstructorResponse>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<IReadOnlyList<InstructorResponse>>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Instructor Async
        public async Task<ApplicationServiceResult<InstructorResponse>> GetInstructorAsync(int id)
        {
            var errorMessage = "There is no Instructor with this id";
            var succeededMessage = "this Instructor details";
            var loggerError = "There is a problem in database";

            try
            {
                var instructor = await _unitOfWork.CreateRepository<Instructor>().GetAsyncSpec(new InstructorSpec(id));
                if (instructor is null) return ApplicationServiceResult<InstructorResponse>.Fail(errorMessage);

                var data = _mapper.Map<InstructorResponse>(instructor);

                return ApplicationServiceResult<InstructorResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<InstructorResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Get All Courses Async
        public async Task<ApplicationServiceResult<Pagination<CourseResponseForInstructor>>> GetAllCoursesAsync(CoursesParams @params)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No Courses For this Instructor";
            var succeededMessage = "this all courses";
            var loggerError = "There is a problem in database";

            try
            {
                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<Pagination<CourseResponseForInstructor>>.Fail(userNotFoundMessage);

                var courseRepo = _unitOfWork.CreateRepository<Course>();
                var spec = new CourseWithInstructorSpec(userId, @params);
                var courses = await courseRepo.GetAllAsyncSpec(spec);
                var courseCount = await courseRepo.GetCountAsyncSpec(new CoursesCountWithSpec(@params));
                if (!courses.Any()) return ApplicationServiceResult<Pagination<CourseResponseForInstructor>>.Fail(errorMessage);

                var dataMapping = _mapper.Map<IReadOnlyList<CourseResponseForInstructor>>(courses);
                var data = new Pagination<CourseResponseForInstructor>(
                        @params.PageIndex,
                        @params.PageSize,
                        courseCount,
                        dataMapping
                    );

                return ApplicationServiceResult<Pagination<CourseResponseForInstructor>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<Pagination<CourseResponseForInstructor>>.Fail(loggerError);
            }
        }
        #endregion
    }
}
