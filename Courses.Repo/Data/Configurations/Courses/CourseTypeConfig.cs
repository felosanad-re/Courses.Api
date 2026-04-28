using Courses.Core.Models.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Courses
{
    public class CourseTypeConfig : IEntityTypeConfiguration<CourseType>
    {
        public void Configure(EntityTypeBuilder<CourseType> builder)
        {
            // Table name
            builder.ToTable("CourseTypes");

            // Primary key
            builder.HasKey(ct => ct.Id);

            // Properties
            builder.Property(ct => ct.Name)
                .IsRequired()
                .HasMaxLength(100);

            // One-to-many relationship: CourseType -> Courses
            builder.HasMany(ct => ct.Courses)
                .WithOne(c => c.CourseType)
                .HasForeignKey(c => c.CourseTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique index on Name — no duplicate course types
            builder.HasIndex(ct => ct.Name)
                .IsUnique();
        }
    }
}
