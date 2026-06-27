using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Students;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;

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

        // Get Students For Current Instructor
        Task<ApplicationServiceResult<Pagination<StudentWithInstructorResponse>>> GetStudentsInstructorAsync(StudentParams @params);

        // Get Student Details
        Task<ApplicationServiceResult<StudentWithInstructorResponse>> GetStudentInstructorAsync(int id);

        // Get Instructor Courses with Student Enrollments
        Task<ApplicationServiceResult<Pagination<InstructorWithCoursesResponse>>> GetMyCoursesAsync(CoursesParams param);

        /// <summary>
        /// Get Online Courses
        /// </summary>
        Task<ApplicationServiceResult<IReadOnlyList<CourseTypesResponse>>> GetOnlineCoursesAsync(string? search);

        /// <summary>
        /// Get Recorded Courses
        /// </summary>
        Task<ApplicationServiceResult<IReadOnlyList<CourseTypesResponse>>> GetRecordedCoursesAsync(string? search);

        /// <summary>
        /// Get Sections For Course By Id [Online | Recorded]
        /// </summary>
        Task<ApplicationServiceResult<IReadOnlyList<SectionListResponse>>> GetSectionsAsync(int courseId);
    }
}
