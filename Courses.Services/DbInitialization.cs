using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Services.Contract;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courses.Services
{
    public class DbInitialization : IDbInitialize
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitialization(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
            // Create Admin
            var user = new ApplicationUser()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "Super_Admin",
                Email = "Admin@Domain.com",
                Address = "Giza"
            };
            var userEmail = await _userManager.FindByEmailAsync(user.Email);
            if (userEmail is null)
            {
                var result = await _userManager.CreateAsync(user, "Admin1234$");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        throw new Exception(error.Description);
                    }
                }
                await _userManager.AddToRoleAsync(user, Roles.Admin);
            }
        }
    }
}
