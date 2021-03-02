using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignSalesAreaTargetEntityConfiguration : IEntityTypeConfiguration<CampaignSalesAreaTarget>
    {
        public void Configure(EntityTypeBuilder<CampaignSalesAreaTarget> builder)
        {
            builder.ToTable("CampaignSalesAreaTargets");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.SalesArea).HasMaxLength(64);
            builder.HasIndex(e => e.CampaignId);
            builder.HasIndex(e => e.SalesArea);

            builder.HasOne(x => x.SalesAreaGroup).WithOne()
                .HasForeignKey<CampaignSalesAreaTargetGroup>(x => x.CampaignSalesAreaTargetId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Multiparts).WithOne()
                .HasForeignKey(x => x.CampaignSalesAreaTargetId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.CampaignTargets).WithOne()
                .HasForeignKey(x => x.CampaignSalesAreaTargetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
