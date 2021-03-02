using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class
        CampaignTargetStrikeWeightDayPartTimeSliceEntityConfiguration :
            IEntityTypeConfiguration<CampaignTargetStrikeWeightDayPartTimeSlice>
    {
        public void Configure(EntityTypeBuilder<CampaignTargetStrikeWeightDayPartTimeSlice> builder)
        {
            builder.ToTable("CampaignTargetStrikeWeightDayPartTimeSlices");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.FromTime).AsTicks();
            builder.Property(e => e.ToTime).AsTicks();
            builder.Property(e => e.DowPattern).AsStringPattern(DayOfWeek.Sunday).IsRequired();

            builder.HasIndex(e => e.CampaignTargetStrikeWeightDayPartId);
        }
    }
}
