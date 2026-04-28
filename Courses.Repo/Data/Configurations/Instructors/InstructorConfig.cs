using Courses.Core.Models.Instructors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.Instructors
{
    public class InstructorConfig : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            // Table name
            builder.ToTable("Instructors");

            // Primary key (inherited from BaseModel via PersonalBase)
            builder.HasKey(i => i.Id);

            // Properties
            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Birthday)
                .IsRequired();

            // Age is computed at runtime — don't map to database
            builder.Ignore(i => i.Age);

            builder.Property(i => i.Specialization)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.UserId)
                .IsRequired();

            // BaseModel properties
            builder.Property(i => i.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(i => i.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // One-to-one relationship with ApplicationUser via UserId
            builder.HasOne(i => i.ApplicationUser)
                .WithOne()
                .HasForeignKey<Instructor>(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many relationship: Instructor -> Courses
            builder.HasMany(i => i.Courses)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Soft delete query filter — automatically exclude deleted instructors
            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
