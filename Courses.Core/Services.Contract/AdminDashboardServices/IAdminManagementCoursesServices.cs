using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.Services.Contract.AdminDashboardServices
{
    public interface IAdminManagementCoursesServices
    {
        Task<ApplicationServiceResult<Pagination<AdminCoursesResponse>>> GetAllCoursesAsync(CoursesParams param, CourseType courseType);

        Task<ApplicationServiceResult<CourseDetailsToReturnDTO>> GetCourseDetailsAsync(int courseId, CourseType type);

        // Change Course Status
        Task<ApplicationServiceResult<bool>> UpdateCourseStatusAsync(int courseId, UpdateCourseStatusRequest req);

        // Delete Course
        Task<ApplicationServiceResult<bool>> DeleteCourseAsync(int courseId);
    }
}
