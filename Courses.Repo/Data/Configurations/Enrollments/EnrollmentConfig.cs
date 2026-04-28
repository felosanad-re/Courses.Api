using Courses.Core.Models.Enrollments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Enrollments
{
    public class EnrollmentConfig : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Table name
            builder.ToTable("Enrollments");

            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.EnrolledAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.Progress)
                .IsRequired()
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5,2)");

            // Many-to-one relationship: Enrollment -> Student
            builder.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-one relationship: Enrollment -> Course
            // Restrict: preserve enrollment records when a course is deleted
            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many relationship: Enrollment -> StudentLectureProgress
            builder.HasMany(e => e.LectureProgresses)
                .WithOne(slp => slp.Enrollment)
                .HasForeignKey(slp => slp.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique index — a student can only enroll once per course
            builder.HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();
        }
    }
}
