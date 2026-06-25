using Courses.Core.Models.Courses;

namespace Courses.Repo.Data.DataSeeding
{
    public static class CoursesDbContextSeeder
    {
        public static async Task SeederAsync(CoursesDbContext dbContext)
        {
            await SeederHelper.SeederFromJSONAsync<CourseCategory>(dbContext, "CoursesType.json");
            await SeederHelper.SeederFromJSONAsync<Course>(dbContext, "Courses.json");
        }
    }
}
