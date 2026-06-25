using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.CoursesCategories;
using Courses.Core.Services.Contract.CourseCategoriesServices;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.CourseCategoriesServices
{
    public class CourseCategoryService : ICourseCategoryService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<CourseCategoryService> _logger;
        protected readonly IMapper _mapper;
        public CourseCategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CourseCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get All Types Async
        public async Task<ApplicationServiceResult<IReadOnlyList<CourseCategoryToReturnDTO>>> GetAllCategoriesAsync()
        {
            try
            {
                var types = await _unitOfWork.CreateRepository<CourseCategory>().GetAllAsync();
                if (!types.Any()) return ApplicationServiceResult<IReadOnlyList<CourseCategoryToReturnDTO>>.Fail("No Course types to show");

                var data = _mapper.Map<IReadOnlyList<CourseCategoryToReturnDTO>>(types);

                return ApplicationServiceResult<IReadOnlyList<CourseCategoryToReturnDTO>>.Success(data, "this all courses types");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when get all courses type form database");
                return ApplicationServiceResult<IReadOnlyList<CourseCategoryToReturnDTO>>.Fail("there is a problem in database");
            }
        }
        #endregion
    }
}
