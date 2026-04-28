using Courses.Core.Models.Enrollments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Enrollments
{
    public class LectureConfig : IEntityTypeConfiguration<Lecture>
    {
        public void Configure(EntityTypeBuilder<Lecture> builder)
        {
            // Table name
            builder.ToTable("Lectures");

            // Primary key
            builder.HasKey(l => l.Id);

            // Properties
            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(l => l.VideoUrl)
                .IsRequired()
                .HasMaxLength(1000);

            // Many-to-one relationship: Lecture -> Section
            builder.HasOne(l => l.Section)
                .WithMany(s => s.Lectures)
                .HasForeignKey(l => l.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship: Lecture -> StudentLectureProgress
            // Restrict: avoid SQL Server multiple cascade paths error
            builder.HasMany(l => l.StudentProgresses)
                .WithOne(slp => slp.Lecture)
                .HasForeignKey(slp => slp.LectureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
