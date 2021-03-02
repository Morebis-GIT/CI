using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignBreakRequirementEntityConfiguration : IEntityTypeConfiguration<CampaignBreakRequirement>
    {
        public void Configure(EntityTypeBuilder<CampaignBreakRequirement> builder)
        {
            _ = builder.ToTable("CampaignBreakRequirements");

            _ = builder.HasKey(e => e.Id);

            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.HasOne(x => x.SalesArea).WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
