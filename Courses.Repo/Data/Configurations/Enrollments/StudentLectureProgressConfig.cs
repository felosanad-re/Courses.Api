using Courses.Core.Models.Enrollments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Enrollments
{
    public class StudentLectureProgressConfig : IEntityTypeConfiguration<StudentLectureProgress>
    {
        public void Configure(EntityTypeBuilder<StudentLectureProgress> builder)
        {
            // Table name
            builder.ToTable("StudentLectureProgresses");

            // Primary key
            builder.HasKey(slp => slp.Id);

            // Properties
            builder.Property(slp => slp.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(slp => slp.CompletedAt)
                .IsRequired(false);

            // Many-to-one relationship: StudentLectureProgress -> Enrollment
            builder.HasOne(slp => slp.Enrollment)
                .WithMany(e => e.LectureProgresses)
                .HasForeignKey(slp => slp.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-one relationship: StudentLectureProgress -> Lecture
            // Restrict: avoid SQL Server multiple cascade paths error
            // (Course->Section->Lecture and Course->Enrollment->StudentLectureProgress)
            builder.HasOne(slp => slp.Lecture)
                .WithMany(l => l.StudentProgresses)
                .HasForeignKey(slp => slp.LectureId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique index — one progress record per enrollment per lecture
            builder.HasIndex(slp => new { slp.EnrollmentId, slp.LectureId })
                .IsUnique();
        }
    }
}
