using AutoMapper;
using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminServices;
using Courses.Core.Services.Contract.UserServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminServices
{
    public class CurrentAdminService : ICurrentAdminService
    {
        protected readonly ICurrentUserService _currentUserService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly ILogger<CurrentAdminService> _logger;

        public CurrentAdminService(ICurrentUserService currentUserService, ILogger<CurrentAdminService> logger, UserManager<ApplicationUser> userManager)
        {
            _currentUserService = currentUserService;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<ApplicationServiceResult<AdminDetailsResponse>> GetCurrentAdmin()
        {
            const string userNotFoundError = "User Not Found With this id";
            const string errorMessage = "there is no admin with this id";
            const string succeededMessage = "Retrieved admin successfully";
            const string loggerError = "there is a problem in database";
            string? userId = null;
            try
            {
                userId = _currentUserService.UserId;
                if (string.IsNullOrWhiteSpace(userId))
                    return ApplicationServiceResult<AdminDetailsResponse>.Fail(userNotFoundError);

                var admin = await _userManager.FindByIdAsync(userId);
                if (admin is null)
                    return ApplicationServiceResult<AdminDetailsResponse>.Fail(errorMessage);
                // check if he is admin
                var isAdmin = await _userManager.IsInRoleAsync(admin, Roles.Admin);
                if (!isAdmin)
                    return ApplicationServiceResult<AdminDetailsResponse>.Fail("Unauthorized");

                var data = new AdminDetailsResponse
                {
                    Id = admin.Id,
                    Name = admin.UserName
                };

                return ApplicationServiceResult<AdminDetailsResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to get details for userId {userId}", userId);
                return ApplicationServiceResult<AdminDetailsResponse>.Fail(loggerError);
            }
        }
    }
}
