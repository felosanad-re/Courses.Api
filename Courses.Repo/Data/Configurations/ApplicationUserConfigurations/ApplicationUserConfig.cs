using Courses.Core.Models.ApplicationUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Repo.Data.Configurations.ApplicationUserConfigurations
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.Status)
                .HasConversion<string>()
                .HasDefaultValue(AccountStatus.Active);
        }
    }
}
