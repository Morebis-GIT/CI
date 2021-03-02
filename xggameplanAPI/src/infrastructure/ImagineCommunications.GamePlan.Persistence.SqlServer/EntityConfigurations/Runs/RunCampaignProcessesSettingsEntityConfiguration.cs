using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunCampaignProcessesSettingsEntityConfiguration : IEntityTypeConfiguration<RunCampaignProcessesSettings>
    {
        public void Configure(EntityTypeBuilder<RunCampaignProcessesSettings> builder)
        {
            builder.ToTable("RunCampaignProcessesSettings");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();
            builder.Property(p => p.ExternalId).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.RunId);
            builder.HasIndex(e => e.ExternalId);
        }
    }
}
