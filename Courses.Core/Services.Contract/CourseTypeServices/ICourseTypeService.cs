using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.CoursesTypes;

namespace Courses.Core.Services.Contract.CourseTypeServices
{
    public interface ICourseTypeService
    {
        Task<ApplicationServiceResult<IReadOnlyList<CourseTypeToReturnDTO>>> GetAllTypesAsync();

        // Add Course Type --> Instructor when create new course with new Type
    }
}
