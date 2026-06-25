using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.CoursesCategories;

namespace Courses.Core.Services.Contract.CourseCategoriesServices
{
    public interface ICourseCategoryService
    {
        Task<ApplicationServiceResult<IReadOnlyList<CourseCategoryToReturnDTO>>> GetAllCategoriesAsync();

        // Add Course Type --> Instructor when create new course with new Type
    }
}
