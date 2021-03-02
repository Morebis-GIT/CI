using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignTimeRestrictionEntityConfiguration : IEntityTypeConfiguration<CampaignTimeRestriction>
    {
        public void Configure(EntityTypeBuilder<CampaignTimeRestriction> builder)
        {
            builder.ToTable("CampaignTimeRestrictions");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.StartDateTime).AsUtc();
            builder.Property(e => e.EndDateTime).AsUtc();
            builder.Property(e => e.DowPattern).AsStringPattern(DayOfWeek.Sunday).IsRequired();

            builder.HasIndex(e => e.CampaignId);

            builder.HasMany(x => x.SalesAreas).WithOne().HasForeignKey(x => x.CampaignTimeRestrictionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
