using Courses.Core.Models.LiveSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.LiveSessions
{
    public class LiveSessionConfig : IEntityTypeConfiguration<LiveSession>
    {
        public void Configure(EntityTypeBuilder<LiveSession> builder)
        {
            builder.HasOne(s => s.Course)
                .WithMany()
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Can't delete Courses if has Sessions

            builder.Property(s => s.ScheduledAt).IsRequired();

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(s => s.ZoomMeetingId)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.RecordingUrl)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
