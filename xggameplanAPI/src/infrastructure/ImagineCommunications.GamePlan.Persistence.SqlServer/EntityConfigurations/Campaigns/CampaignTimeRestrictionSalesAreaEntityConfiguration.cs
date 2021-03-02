using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignTimeRestrictionSalesAreaEntityConfiguration : IEntityTypeConfiguration<CampaignTimeRestrictionSalesArea>
    {
        public void Configure(EntityTypeBuilder<CampaignTimeRestrictionSalesArea> builder)
        {
            builder.ToTable("CampaignTimeRestrictionsSalesAreas");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.CampaignTimeRestrictionId);
            builder.HasIndex(e => e.Name);
        }
    }
}
