using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ScenarioCampaignFailureEntityConfiguration : IEntityTypeConfiguration<ScenarioCampaignFailure>
    {
        public void Configure(EntityTypeBuilder<ScenarioCampaignFailure> builder)
        {
            builder.ToTable("ScenarioCampaignFailures");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();

            builder.Property(p => p.ExternalCampaignId).HasMaxLength(64);
            builder.Property(p => p.SalesArea).HasMaxLength(64);
            builder.Property(e => e.Length).AsTicks();
            builder.Property(e => e.StrikeWeightStartDate).AsUtc();
            builder.Property(e => e.StrikeWeightEndDate).AsUtc();
            builder.Property(e => e.DayPartStartTime).AsTicks();
            builder.Property(e => e.DayPartEndTime).AsTicks();
            builder.Property(p => p.DayPartDays).HasMaxLength(7);
            builder.Property(p => p.SalesAreaGroup).HasMaxLength(64);
            builder.Property(p => p.PassesEncounteringFailure).HasMaxLength(128);

            builder.HasIndex(p => p.ScenarioId);
            builder.HasIndex(p => p.ExternalCampaignId);
            builder.HasIndex(p => p.SalesArea);
            builder.HasIndex(p => p.StrikeWeightStartDate);
            builder.HasIndex(p => p.StrikeWeightEndDate);
            builder.HasIndex(p => p.SalesAreaGroup);
        }
    }
}
