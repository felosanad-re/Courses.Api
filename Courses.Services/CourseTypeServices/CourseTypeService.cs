using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.CoursesTypes;
using Courses.Core.Services.Contract.CourseTypeServices;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.CourseTypeServices
{
    public class CourseTypeService : ICourseTypeService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<CourseTypeService> _logger;
        protected readonly IMapper _mapper;
        public CourseTypeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CourseTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get All Types Async
        public async Task<ApplicationServiceResult<IReadOnlyList<CourseTypeToReturnDTO>>> GetAllTypesAsync()
        {
            try
            {
                var types = await _unitOfWork.CreateRepository<CourseType>().GetAllAsync();
                if (!types.Any()) return ApplicationServiceResult<IReadOnlyList<CourseTypeToReturnDTO>>.Fail("No Course types to show");

                var data = _mapper.Map<IReadOnlyList<CourseTypeToReturnDTO>>(types);

                return ApplicationServiceResult<IReadOnlyList<CourseTypeToReturnDTO>>.Success(data, "this all courses types");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when get all courses type form database");
                return ApplicationServiceResult<IReadOnlyList<CourseTypeToReturnDTO>>.Fail("there is a problem in database");
            }
        }
        #endregion
    }
}
