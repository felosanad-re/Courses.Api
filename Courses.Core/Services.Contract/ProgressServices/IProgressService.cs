using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Progress;
using Courses.Core.ModelsDTO.ResponseDTO.Progress;

namespace Courses.Core.Services.Contract.ProgressServices
{
    public interface IProgressService
    {
        // Get Last Watch sec
        Task<ApplicationServiceResult<ProgressWithLectureResponse>> GetCurrentLectureProgressAsync(int lectureId);

        // update and add last watched sec
        Task<ApplicationServiceResult<ProgressWithLectureResponse>> UpdateAndAddLastWatchedAsync(UpdateAndAddProgressRequest req);

        // Get Course progress [Full Course]
        Task<ApplicationServiceResult<CourseProgressResponse>> GetCourseProgressAsync(int courseId);
    }
}
