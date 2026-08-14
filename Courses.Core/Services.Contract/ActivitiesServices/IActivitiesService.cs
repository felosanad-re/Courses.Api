using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;

namespace Courses.Core.Services.Contract.ActivitiesServices
{
    public interface IActivitiesService
    {
        Task<ApplicationServiceResult<IReadOnlyList<InstructorActivitiesResponse>>> GetActivitiesAsync();
    }
}
