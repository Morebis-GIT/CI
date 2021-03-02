using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignTargetEntityConfiguration : IEntityTypeConfiguration<CampaignTarget>
    {
        public void Configure(EntityTypeBuilder<CampaignTarget> builder)
        {
            builder.ToTable("CampaignTargets");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.HasIndex(e => e.CampaignSalesAreaTargetId);

            builder.HasMany(x => x.StrikeWeights).WithOne().HasForeignKey(x => x.CampaignTargetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
