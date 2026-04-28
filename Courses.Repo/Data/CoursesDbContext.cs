using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;
using Courses.Core.Models.Students;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Courses.Repo.Data
{
    public class CoursesDbContext : IdentityDbContext<ApplicationUser>
    {
        public CoursesDbContext(DbContextOptions<CoursesDbContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        // Students
        public DbSet<Student> Students { get; set; }

        // Instructors
        public DbSet<Instructor> Instructors { get; set; }

        // Courses
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseType> CourseTypes { get; set; }

        // Enrollments
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<StudentLectureProgress> StudentLectureProgresses { get; set; }
    }
}
