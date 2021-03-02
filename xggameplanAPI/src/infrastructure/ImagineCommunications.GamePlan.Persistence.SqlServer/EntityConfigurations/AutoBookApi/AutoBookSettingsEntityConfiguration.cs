using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi
{
    public class AutoBookSettingsEntityConfiguration : IEntityTypeConfiguration<AutoBookSettings>
    {
        public void Configure(EntityTypeBuilder<AutoBookSettings> builder)
        {
            builder.ToTable("AutoBookSettings");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.MinLifetime).AsTicks();
            builder.Property(e => e.MaxLifetime).AsTicks();
            builder.Property(e => e.CreationTimeout).AsTicks();

            builder.Property(e => e.ProvisioningAPIURL).HasMaxLength(256);
            builder.Property(e => e.ApplicationVersion).HasMaxLength(256);
            builder.Property(e => e.BinariesVersion).HasMaxLength(256);
        }
    }
}
