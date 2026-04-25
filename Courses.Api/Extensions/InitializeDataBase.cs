using Courses.Core.Services.Contract;
using Courses.Repo.Data;
using Microsoft.EntityFrameworkCore;

namespace Courses.Api.Extensions
{
    public static class InitializeDataBase
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            // Add Scope
            using var scope = app.Services.CreateScope();
            // Add Services
            var services = scope.ServiceProvider;
            // Create object From DbContext Implicitly
            var _context = services.GetRequiredService<CoursesDbContext>();
            // Create object From DbInitialize Implicitly
            var _dbInitialization = services.GetRequiredService<IDbInitialize>();
            var logger = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _context.Database.MigrateAsync();
                await _dbInitialization.CreateInitializationAsync();
                //await AdminDbContextSeeder.SeederAsync(_context);
            }
            catch (Exception ex)
            {
                var _logger = logger.CreateLogger<Program>();
                _logger.LogError(ex, "Error in database");
            }
        }
    }
}
