using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.Services.Contract.CoursesServices
{
    public interface ICourseService
    {
        Task<ApplicationServiceResult<Pagination<CoursesToReturnDTO>>> GetAllCoursesAsync(CoursesParams @params);

        Task<ApplicationServiceResult<CourseDetailsToReturnDTO>> GetCourseDetailsAsync(int courseId);
    }
}
