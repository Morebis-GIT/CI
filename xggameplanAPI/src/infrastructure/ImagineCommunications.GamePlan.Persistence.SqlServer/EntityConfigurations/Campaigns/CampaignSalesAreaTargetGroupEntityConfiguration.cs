using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignSalesAreaTargetGroupEntityConfiguration : IEntityTypeConfiguration<CampaignSalesAreaTargetGroup>
    {
        public void Configure(EntityTypeBuilder<CampaignSalesAreaTargetGroup> builder)
        {
            builder.ToTable("CampaignSalesAreaTargetGroups");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.GroupName).HasMaxLength(128);

            builder.HasIndex(e => e.CampaignSalesAreaTargetId);

            builder.HasMany(x => x.SalesAreas).WithOne().HasForeignKey(x => x.CampaignSalesAreaTargetGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
