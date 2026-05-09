using Courses.Core.Models.Instructors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.InstructorRequestConfigurations
{
    public class InstructorRequestConfig : IEntityTypeConfiguration<InstructorRequest>
    {
        public void Configure(EntityTypeBuilder<InstructorRequest> builder)
        {
            builder.Property(I => I.Status)
                .HasConversion<string>()
                .HasDefaultValue(InstructorRequestStatus.Pending);

            builder.Property(I => I.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.Property(I => I.Bio).HasMaxLength(1000);
            builder.Property(I => I.Specialty).HasMaxLength(100);
            builder.HasIndex(I => I.UserId);

            builder.HasIndex(I => I.Status);

            builder.HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
