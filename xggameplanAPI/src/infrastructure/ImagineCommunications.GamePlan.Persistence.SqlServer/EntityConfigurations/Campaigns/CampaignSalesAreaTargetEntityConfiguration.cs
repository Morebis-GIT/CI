using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignSalesAreaTargetEntityConfiguration : IEntityTypeConfiguration<CampaignSalesAreaTarget>
    {
        public void Configure(EntityTypeBuilder<CampaignSalesAreaTarget> builder)
        {
            _ = builder.ToTable("CampaignSalesAreaTargets");

            _ = builder.HasKey(e => e.Id);

            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.HasIndex(e => e.CampaignId);
            _ = builder.HasIndex(e => e.SalesAreaId);

            _ = builder.HasOne(x => x.SalesAreaGroup).WithOne()
                .HasForeignKey<CampaignSalesAreaTargetGroup>(x => x.CampaignSalesAreaTargetId)
                .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasMany(x => x.Multiparts).WithOne()
                .HasForeignKey(x => x.CampaignSalesAreaTargetId)
                .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasMany(x => x.CampaignTargets).WithOne()
                .HasForeignKey(x => x.CampaignSalesAreaTargetId)
                .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasOne(x => x.SalesArea).WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
