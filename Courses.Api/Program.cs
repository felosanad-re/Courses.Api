using Courses.Api.ErrorHandler;
using Courses.Api.Extensions;
using Courses.Core.Models.ApplicationUsers;
using Courses.Repo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Courses.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                #region Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();
                builder.Services.AddSwaggerGen();

                // Add Application Services
                builder.Services.AddApplicationServices(builder.Configuration);
                // Add DbContext
                builder.Services.AddDbContext<CoursesDbContext>(options =>
                {
                    options.UseSqlServer(BuildConnectionString(builder.Configuration));
                });
                // Add Identity
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 5;
                }).AddEntityFrameworkStores<CoursesDbContext>().AddDefaultTokenProviders();

                #endregion


                var app = builder.Build();

                await app.InitializeDatabaseAsync();
                app.UseMiddleware<ExceptionMiddleware>();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseStaticFiles();
                app.UseAuthentication();
                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                WriteStartupFailure(ex);
                throw;
            }
        }

        #region Build Connection String
        private static string BuildConnectionString(IConfiguration configuration)
        {
            var configuredConnectionString = configuration.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(configuredConnectionString))
            {
                throw new InvalidOperationException("Connection string 'Default' is missing.");
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder(configuredConnectionString);
            var passwordFromSecret = configuration["ConnectionStrings:DefaultPassword"];

            if (string.IsNullOrWhiteSpace(connectionStringBuilder.Password))
            {
                var passwordFromFallbackProvider = TryGetPasswordFromFallbackConnectionString(configuration);
                if (!string.IsNullOrWhiteSpace(passwordFromFallbackProvider))
                {
                    connectionStringBuilder.Password = passwordFromFallbackProvider;
                }
            }

            if (string.IsNullOrWhiteSpace(connectionStringBuilder.Password))
            {
                var passwordFromProductionFile = TryGetPasswordFromJsonFile(configuration, "appsettings.Production.json");
                if (!string.IsNullOrWhiteSpace(passwordFromProductionFile))
                {
                    connectionStringBuilder.Password = passwordFromProductionFile;
                }
            }

            if (!string.IsNullOrWhiteSpace(passwordFromSecret))
            {
                connectionStringBuilder.Password = passwordFromSecret;
            }

            if (!connectionStringBuilder.IntegratedSecurity && string.IsNullOrWhiteSpace(connectionStringBuilder.Password))
            {
                throw new InvalidOperationException("Database password is missing. Set 'ConnectionStrings__DefaultPassword' or provide a full 'ConnectionStrings__Default' value outside git.");
            }

            return connectionStringBuilder.ConnectionString;
        }

        private static string? TryGetPasswordFromFallbackConnectionString(IConfiguration configuration)
        {
            if (configuration is not IConfigurationRoot configurationRoot)
            {
                return null;
            }

            foreach (var provider in configurationRoot.Providers)
            {
                if (!provider.TryGet("ConnectionStrings:Default", out var candidateConnectionString) ||
                    string.IsNullOrWhiteSpace(candidateConnectionString))
                {
                    continue;
                }

                try
                {
                    var candidateBuilder = new SqlConnectionStringBuilder(candidateConnectionString);
                    if (!string.IsNullOrWhiteSpace(candidateBuilder.Password))
                    {
                        return candidateBuilder.Password;
                    }
                }
                catch (ArgumentException)
                {
                    // Ignore malformed connection strings from optional providers and keep searching.
                }
            }

            return null;
        }

        private static string? TryGetPasswordFromJsonFile(IConfiguration configuration, string fileName)
        {
            var contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            if (string.IsNullOrWhiteSpace(contentRoot))
            {
                contentRoot = AppContext.BaseDirectory;
            }

            var fullPath = Path.Combine(contentRoot, fileName);
            if (!File.Exists(fullPath))
            {
                return null;
            }

            try
            {
                var directConfiguration = new ConfigurationBuilder()
                    .SetBasePath(contentRoot)
                    .AddJsonFile(fileName, optional: false, reloadOnChange: false)
                    .Build();

                var candidateConnectionString = directConfiguration.GetConnectionString("Default");
                if (string.IsNullOrWhiteSpace(candidateConnectionString))
                {
                    return null;
                }

                var candidateBuilder = new SqlConnectionStringBuilder(candidateConnectionString);
                return string.IsNullOrWhiteSpace(candidateBuilder.Password) ? null : candidateBuilder.Password;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Write Error in log file
        private static void WriteStartupFailure(Exception ex)
        {
            var message = $"""
            [{DateTime.UtcNow:O}] Application startup failed.
            {FlattenException(ex)}

            """;

            foreach (var logPath in GetStartupLogPaths())
            {
                try
                {
                    var directory = Path.GetDirectoryName(logPath);
                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.AppendAllText(logPath, message);
                }
                catch
                {
                    // Try the next writable location.
                }
            }

            try
            {
                Console.Error.WriteLine(message);
            }
            catch
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private static IEnumerable<string> GetStartupLogPaths()
        {
            var paths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "startup-error.log"),
                Path.Combine(AppContext.BaseDirectory, "logs", "startup-error.log"),
                Path.Combine(Path.GetTempPath(), "Courses.Api", "startup-error.log")
            };

            return paths.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static string FlattenException(Exception ex)
        {
            var lines = new List<string>();
            var current = ex;
            var level = 0;

            while (current != null)
            {
                lines.Add($"Level {level}: {current.GetType().FullName}");
                lines.Add($"Message: {current.Message}");
                lines.Add(current.StackTrace ?? "No stack trace available.");
                lines.Add(string.Empty);
                current = current.InnerException!;
                level++;
            }

            return string.Join(Environment.NewLine, lines);
        }
        #endregion
    }
}

