using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignProgrammeRestrictionSalesAreaEntityConfiguration : IEntityTypeConfiguration<CampaignProgrammeRestrictionSalesArea>
    {
        public void Configure(EntityTypeBuilder<CampaignProgrammeRestrictionSalesArea> builder)
        {
            _ = builder.ToTable("CampaignProgrammeRestrictionsSalesAreas");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.HasIndex(e => e.CampaignProgrammeRestrictionId);
            _ = builder.HasIndex(e => e.SalesAreaId);

            _ = builder.HasOne(x => x.SalesArea).WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
