using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;

namespace Courses.Core.Services.Contract.AdminServices
{
    public interface ICurrentAdminService
    {
        Task<ApplicationServiceResult<AdminDetailsResponse>> GetCurrentAdmin();
    }
}
