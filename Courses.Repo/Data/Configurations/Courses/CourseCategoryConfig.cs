using Courses.Core.Models.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Courses
{
    public class CourseCategoryConfig : IEntityTypeConfiguration<CourseCategory>
    {
        public void Configure(EntityTypeBuilder<CourseCategory> builder)
        {
            // Table name
            builder.ToTable("CourseCategories");

            // Primary key
            builder.HasKey(ct => ct.Id);

            // Properties
            builder.Property(ct => ct.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ct => ct.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            // One-to-many relationship: CourseType -> Courses
            builder.HasMany(ct => ct.Courses)
                .WithOne(c => c.CourseCategory)
                .HasForeignKey(c => c.CourseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique index on Name — no duplicate course types
            builder.HasIndex(ct => ct.Name)
                .IsUnique();
        }
    }
}
