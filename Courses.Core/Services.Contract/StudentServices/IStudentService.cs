using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.Services.Contract.StudentServices
{
    public interface IStudentService
    {
        // Get All Courses
        Task<ApplicationServiceResult<IReadOnlyList<CoursesToReturnDTO>>> GetAllStudentCoursesAsync();
    }
}
