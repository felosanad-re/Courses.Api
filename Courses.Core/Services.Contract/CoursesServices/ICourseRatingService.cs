using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.Services.Contract.CoursesServices
{
    public interface ICourseRatingService
    {
        Task<ApplicationServiceResult<CourseRatingResponse>> CreateCourseRatingAsync(int courseId, CourseRatingRequest req);
    }
}
