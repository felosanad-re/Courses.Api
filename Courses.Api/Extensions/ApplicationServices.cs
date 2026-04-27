using Courses.Api.Helper.EmailSend;
using Courses.Api.Helper.Mapping;
using Courses.Core.Options;
using Courses.Core.RedisRepository;
using Courses.Core.Services.Contract;
using Courses.Core.Services.Contract.AccountServices;
using Courses.Core.Services.Contract.AttachmentServices;
using Courses.Core.UnitOfWork;
using Courses.Repo.RedisRepository;
using Courses.Repo.UnitOfWorks;
using Courses.Services;
using Courses.Services.AccountServices;
using Courses.Services.AttachmentServices;
using Courses.Services.CreateToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Courses.Api.Extensions
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICreateToken, CreateToken>();
            services.AddScoped<IDbInitialize, DbInitialization>();
            services.AddAutoMapper(typeof(ProfileMapping));
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped(typeof(IRedisRepo<>), typeof(RedisRepo<>));


            var jwtSection = configuration.GetSection(JwtOptions.SectionName);
            services.Configure<JwtOptions>(jwtSection);
            services.Configure<SeedAdminOptions>(configuration.GetSection(SeedAdminOptions.SectionName));

            var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
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
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(jwtOptions.ClockSkewMinutes),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

            return services;
        }
    }
}
