using Courses.Core.Services.Contract;
using Courses.Core.Services.Contract.AttachmentServices;
using Courses.Core.UnitOfWork;
using Courses.Repo.UnitOfWorks;
using Courses.Services;
using Courses.Services.AttachmentServices;
using Courses.Services.CreateToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Courses.Api.Extensions
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _configuration)
        {
            services.AddScoped<ICreateToken, CreateToken>();
            services.AddScoped<IDbInitialize, DbInitialization>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            #region Add JWT Token
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:audience"],
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromDays(double.Parse(_configuration["JWT:expires"])),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]))
                };
            });
            #endregion
            return services;
        }
    }
}
