using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;

namespace Courses.Core.Services.Contract.CoursesServices
{
    public interface IVideoCourseService
    {
        // Service For Paly Video In Lecture
        Task<ApplicationServiceResult<CourseWithLectureVideoResponse>> GetLectureVideo(int lectureId);

        Task<ApplicationServiceResult<CourseWithLectureVideoResponse>> GetNextLectureVideo(int sectionId, int currentVideoOrder);

        Task<ApplicationServiceResult<CourseWithLectureVideoResponse>> GetPreviousLectureVideo(int sectionId, int currentVideoOrder);
    }
}
