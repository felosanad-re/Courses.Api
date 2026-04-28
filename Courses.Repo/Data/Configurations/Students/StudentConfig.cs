using Courses.Core.Models.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Students
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Table name
            builder.ToTable("Students");

            // Primary key (inherited from BaseModel via PersonalBase)
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Birthday)
                .IsRequired();

            // Age is computed at runtime — don't map to database
            builder.Ignore(s => s.Age);

            builder.Property(s => s.UserId)
                .IsRequired();

            // BaseModel properties
            builder.Property(s => s.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // One-to-one relationship with ApplicationUser via UserId
            builder.HasOne(s => s.ApplicationUser)
                .WithOne()
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many relationship: Student -> Enrollments
            builder.HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Soft delete query filter — automatically exclude deleted students
            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
