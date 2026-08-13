using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Account.AdminManagementAccounts;

namespace Courses.Core.Services.Contract.AdminDashboardServices
{
    public interface IAdminCreateUser
    {
        // Create User and set roles [Instructor - Admin]
        Task<ApplicationServiceResult<bool>> CreateUserAsync(AdminCreateUserReq req);
    }
}
