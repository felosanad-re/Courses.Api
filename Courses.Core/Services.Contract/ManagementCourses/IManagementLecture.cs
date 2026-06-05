using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;

namespace Courses.Core.Services.Contract.ManagementCourses
{
    public interface IManagementLecture
    {
        // Create Lecture
        Task<ApplicationServiceResult<LectureWithInstructorResponse>> CreateLectureAsync(CreatedLectureRequest req);

        // Get Lecture
        Task<ApplicationServiceResult<LectureWithInstructorResponse>> GetLectureAsync(int id);

        // Edit Lecture
        Task<ApplicationServiceResult<LectureWithInstructorResponse>> UpdateLectureAsync(UpdatedLectureRequest req);

        // Delete Lecture
        Task<ApplicationServiceResult<LectureDeletedResponse>> DeleteLectureAsync(int id);

        // Multi-Delete Lectures
        Task<ApplicationServiceResult<LectureDeletedResponse>> DeleteMultiLectureAsync(IEnumerable<int> ids);
    }
}
