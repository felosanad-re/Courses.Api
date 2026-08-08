using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Account.AdminManagementAccounts;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminManagementAccountService : IAdminManagementAccountService
    {
        private const string AccountNotFoundMessage = "Account Not Found";
        private const string UpdateFailedMessage = "Failed to update account.";

        #region Services
        protected readonly ICurrentUserService _currentUserService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly ILogger<AdminManagementAccountService> _logger;

        public AdminManagementAccountService(ICurrentUserService currentUserService, ILogger<AdminManagementAccountService> logger, UserManager<ApplicationUser> userManager)
        {
            _currentUserService = currentUserService;
            _logger = logger;
            _userManager = userManager;
        }
        #endregion

        #region Activate Account Async
        public async Task<ApplicationServiceResult<bool>> ActivateAccountAsync(string userId)
        {
            const string SucceededMessage = "Account Activated Succeeded";
            const string WarningMassage = "Account Is Already Activated";
            const string LoggerMassage = "There is a problem in database";
            string? adminId = null;

            try
            {
                adminId = _currentUserService.UserId;
                var adminName = _currentUserService.UserName;
                return await ChangeStatusAsync(userId,
                    AccountStatus.Active,
                    SucceededMessage,
                    WarningMassage,
                    adminName!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to Activated Account userId {userId} by adminId {adminId}", userId, adminId);
                return ApplicationServiceResult<bool>.Fail(LoggerMassage);
            }
        }
        #endregion

        #region Suspend Account Async
        public async Task<ApplicationServiceResult<bool>> SuspendAccountAsync(AccountActionRequest req, string userId)
        {
            const string SucceededMessage = "Account Suspended Succeeded";
            const string WarningMassage = "Account Is Already Suspended";
            const string LoggerMassage = "There is a problem in database";
            string? adminId = null;

            try
            {
                adminId = _currentUserService.UserId;
                var adminName = _currentUserService.UserName;
                return await ChangeStatusAsync(userId,
                    AccountStatus.Suspended,
                    SucceededMessage,
                    WarningMassage,
                    adminName!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to Suspended Account userId {userId} by adminId {adminId}", userId, adminId);
                return ApplicationServiceResult<bool>.Fail(LoggerMassage);
            }
        }
        #endregion

        #region Delete Account Async
        public async Task<ApplicationServiceResult<bool>> DeleteAccountAsync(AccountActionRequest req, string userId)
        {
            const string SucceededMessage = "Account Deleted Succeeded";
            const string WarningMassage = "Account Is Already Deleted";
            const string LoggerMassage = "There is a problem in database";
            string? adminId = null;

            try
            {
                var adminName = _currentUserService?.UserName;
                return await UpdateDeletedAsync(userId, SucceededMessage, WarningMassage, adminName!, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to Suspended Account userId {userId} by adminId {adminId}", userId, adminId);
                return ApplicationServiceResult<bool>.Fail(LoggerMassage);
            }
        }
        #endregion

        #region Restore Account Async
        public async Task<ApplicationServiceResult<bool>> RestoreAccountAsync(string userId)
        {
            const string SucceededMessage = "Account Restored Succeeded";
            const string WarningMassage = "Account Is Already Exist";
            const string LoggerMassage = "There is a problem in database";
            string? adminId = null;

            try
            {
                var adminName = _currentUserService?.UserName;
                return await UpdateDeletedAsync(userId, SucceededMessage, WarningMassage, adminName!, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to Suspended Account userId {userId} by adminId {adminId}", userId, adminId);
                return ApplicationServiceResult<bool>.Fail(LoggerMassage);
            }
        }
        #endregion

        #region Helper Methods
        private async Task<ApplicationServiceResult<bool>> ChangeStatusAsync(string userId, AccountStatus newStatus, string succeededMessage, string warningMessage, string adminName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApplicationServiceResult<bool>.Fail("Account Not Found");

            if (user.Status == newStatus)
                return ApplicationServiceResult<bool>.Fail(warningMessage);

            user.Status = newStatus;
            if(newStatus == AccountStatus.Suspended)
            {
                user.SuspendedAt = DateTime.UtcNow;
                user.SuspendedBy = adminName;
            }
            else
            {
                user.SuspendedAt = null;
                user.SuspendedBy = null;
            }

            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded)
                return ApplicationServiceResult<bool>.Fail(AccountNotFoundMessage);
            return ApplicationServiceResult<bool>.Success(true, succeededMessage);
        }

        private async Task<ApplicationServiceResult<bool>> UpdateDeletedAsync(string userId, string succeededMessage, string warningMessage, string adminName, bool isDeleted)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApplicationServiceResult<bool>.Fail(AccountNotFoundMessage);

            if (user.IsDeleted == isDeleted)
                return ApplicationServiceResult<bool>.Fail(warningMessage);

            user.IsDeleted = isDeleted;

            if(isDeleted)
            {
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedBy = adminName;
                user.Status = AccountStatus.Suspended;
            }
            else
            {
                user.DeletedAt = null;
                user.DeletedBy = null;
                user.Status = AccountStatus.Active;
            }

            var res = await _userManager.UpdateAsync(user);
            if(!res.Succeeded)
                return ApplicationServiceResult<bool>.Fail(UpdateFailedMessage);

            return ApplicationServiceResult<bool>.Success(true, succeededMessage);
        }
        #endregion
    }
}
