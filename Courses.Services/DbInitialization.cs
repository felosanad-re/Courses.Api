using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Options;
using Courses.Core.Services.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Courses.Services
{
    public class DbInitialization : IDbInitialize
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SeedAdminOptions _seedAdminOptions;

        public DbInitialization(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<SeedAdminOptions> seedAdminOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _seedAdminOptions = seedAdminOptions.Value;
        }

        public async Task CreateInitializationAsync()
        {
            string[] rolesName = [Roles.Student, Roles.Admin, Roles.Instructor];

            foreach (var role in rolesName)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (!_seedAdminOptions.Enabled)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_seedAdminOptions.Email) ||
                string.IsNullOrWhiteSpace(_seedAdminOptions.UserName) ||
                string.IsNullOrWhiteSpace(_seedAdminOptions.Password))
            {
                throw new InvalidOperationException("Seed admin is enabled, but the required settings are missing.");
            }

            var existingUser = await _userManager.FindByEmailAsync(_seedAdminOptions.Email);
            if (existingUser is not null)
            {
                return;
            }

            var user = new ApplicationUser
            {
                FirstName = _seedAdminOptions.FirstName,
                LastName = _seedAdminOptions.LastName,
                UserName = _seedAdminOptions.UserName,
                Email = _seedAdminOptions.Email,
                Address = _seedAdminOptions.Address
            };

            var result = await _userManager.CreateAsync(user, _seedAdminOptions.Password);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(Environment.NewLine, result.Errors.Select(error => error.Description));
                throw new InvalidOperationException(errorMessage);
            }

            await _userManager.AddToRoleAsync(user, Roles.Admin);
        }
    }
}