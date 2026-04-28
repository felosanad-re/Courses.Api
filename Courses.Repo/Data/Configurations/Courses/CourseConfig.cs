using Courses.Core.Models.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Courses
{
    public class CourseConfig : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Table name
            builder.ToTable("Courses");

            // Primary key (inherited from BaseModel)
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.Image)
                .HasMaxLength(500);

            builder.Property(c => c.IsPaid)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.Price)
                .IsRequired()
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18,2)");

            // BaseModel properties
            builder.Property(c => c.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Many-to-one relationship: Course -> Instructor
            builder.HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-one relationship: Course -> CourseType
            builder.HasOne(c => c.CourseType)
                .WithMany(ct => ct.Courses)
                .HasForeignKey(c => c.CourseTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many relationship: Course -> Enrollments
            // Restrict: preserve enrollment records when a course is deleted
            // (students should still see the course name in their history)
            builder.HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many relationship: Course -> Sections
            builder.HasMany(c => c.Sections)
                .WithOne(s => s.Course)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Soft delete query filter — automatically exclude deleted courses
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
