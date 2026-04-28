using Courses.Core.Models.Enrollments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Enrollments
{
    public class SectionConfig : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            // Table name
            builder.ToTable("Sections");

            // Primary key
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(300);

            // Many-to-one relationship: Section -> Course
            builder.HasOne(s => s.Course)
                .WithMany(c => c.Sections)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship: Section -> Lectures
            builder.HasMany(s => s.Lectures)
                .WithOne(l => l.Section)
                .HasForeignKey(l => l.SectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
