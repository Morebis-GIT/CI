using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignSalesAreaTargetGroupSalesAreaEntityConfiguration : IEntityTypeConfiguration<CampaignSalesAreaTargetGroupSalesArea>
    {
        public void Configure(EntityTypeBuilder<CampaignSalesAreaTargetGroupSalesArea> builder)
        {
            builder.ToTable("CampaignSalesAreaTargetGroupSalesAreas");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.CampaignSalesAreaTargetGroupId);
            builder.HasIndex(e => e.Name);
        }
    }
}
