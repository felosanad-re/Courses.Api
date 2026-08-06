using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Account.AdminManagementAccounts;

namespace Courses.Core.Services.Contract.AdminDashboardServices
{
    public interface IAdminManagementAccountService
    {
        // Suspended Account
        Task<ApplicationServiceResult<bool>> SuspendAccountAsync(AccountActionRequest req, string userId);

        // Activate Account
        Task<ApplicationServiceResult<bool>> ActivateAccountAsync(string userId);

        // delete account
        Task<ApplicationServiceResult<bool>> DeleteAccountAsync(AccountActionRequest req, string userId);

        // Restore Account
        Task<ApplicationServiceResult<bool>> RestoreAccountAsync(string userId);
    }
}
