using Courses.Core.Models.LiveSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.LiveSessions
{
    public class LiveSessionConfig : IEntityTypeConfiguration<LiveSession>
    {
        public void Configure(EntityTypeBuilder<LiveSession> builder)
        {
            builder.HasOne(s => s.Section)
                .WithMany()
                .HasForeignKey(s => s.SectionId)
                .OnDelete(DeleteBehavior.Restrict); // Can't delete Sections if has Sessions

            builder.Property(s => s.Topic)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.ScheduledAt).IsRequired();

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(s => s.ZoomMeetingId)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.RecordingUrl)
                .HasMaxLength(500);
        }
    }
}
