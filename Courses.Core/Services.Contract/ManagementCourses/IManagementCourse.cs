using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.Services.Contract.ManagementCourses
{
    public interface IManagementCourse
    {
        // Create Course
        Task<ApplicationServiceResult<CourseResponseForInstructor>> CreateCourseAsync(CreatedCourseRequest req);

        // Edit Course
        Task<ApplicationServiceResult<CourseResponseForInstructor>> UpdateCourseAsync(int id, UpdatedCourseRequest req);

        // Get Course With his Instructor
        Task<ApplicationServiceResult<CourseResponseForInstructor>> GetCourseDetailsAsync(int id);

        // Delete Courses
        Task<ApplicationServiceResult<bool>> DeleteCourseAsync(int id);

        // Delete Bulk
        Task<ApplicationServiceResult<DeleteCoursesResult>> DeleteCoursesAsync(IEnumerable<int> courseIds);
    }
}
