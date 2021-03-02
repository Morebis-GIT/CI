using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignSettingsEntityConfiguration : IEntityTypeConfiguration<CampaignSettings>
    {
        public void Configure(EntityTypeBuilder<CampaignSettings> builder)
        {
            builder.ToTable("CampaignSettings");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(e => e.CampaignExternalId);
        }
    }
}
