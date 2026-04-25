using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Services.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Courses.Services.CreateToken
{
    public class CreateToken : ICreateToken
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IConfiguration _configuration;

        public CreateToken(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            // Create Claims
            var CreateClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                CreateClaims.Add(new Claim(ClaimTypes.Role, role));

            // Create Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

            // Create Token Object
            var Token = new JwtSecurityToken(
                    issuer: _configuration["JWT:issuer"],
                    audience: _configuration["JWT:audience"],
                    CreateClaims,
                    expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:expires"]!)),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
