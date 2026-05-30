using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;

namespace Courses.Core.Services.Contract.InstructorServices
{
    public interface IInstructorService
    {
        // Get All Instructor
        Task<ApplicationServiceResult<IReadOnlyList<InstructorResponse>>> GetAllInstructorsAsync();
        // Get Instructor
        Task<ApplicationServiceResult<InstructorResponse>> GetInstructorAsync(int id);
        // Get All Courses For Instructor
        Task<ApplicationServiceResult<Pagination<CourseResponseForInstructor>>> GetAllCoursesAsync(CoursesParams @params);
    }
}
