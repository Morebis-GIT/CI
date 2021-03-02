using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignSalesAreaTargetMultipartLengthEntityConfiguration : IEntityTypeConfiguration<CampaignSalesAreaTargetMultipartLength>
    {
        public void Configure(EntityTypeBuilder<CampaignSalesAreaTargetMultipartLength> builder)
        {
            builder.ToTable("CampaignSalesAreaTargetMultipartLengths");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.Length).AsTicks();

            builder.HasIndex(e => e.CampaignSalesAreaTargetMultipartId);
        }
    }
}
