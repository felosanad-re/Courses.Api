using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.DashboardInstructor;

namespace Courses.Core.Services.Contract.DashboardServices
{
    public interface IDashboardInstructorService
    {
        Task<ApplicationServiceResult<DashboardInstructorDTO>> GetDashboardInstructStatsAsync();
    }
}
