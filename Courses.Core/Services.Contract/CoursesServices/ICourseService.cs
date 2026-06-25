using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.Services.Contract.CoursesServices
{
    public interface ICourseService
    {
        Task<ApplicationServiceResult<Pagination<CoursesToReturnDTO>>> GetAllCoursesAsync(CoursesParams @params);

        /// <summary>
        /// Get Online Courses
        /// </summary>
        //Task<ApplicationServiceResult<IReadOnlyList<CourseStatusResponse>>> GetOnlineCourseStatusAsync();

        /// <summary>
        /// Get Recorded Courses
        /// </summary>
        //Task<ApplicationServiceResult<IReadOnlyList<CourseStatusResponse>>> GetRecordedCourseStatusAsync();

        Task<ApplicationServiceResult<CourseDetailsToReturnDTO>> GetCourseDetailsAsync(int courseId);
    }
}
