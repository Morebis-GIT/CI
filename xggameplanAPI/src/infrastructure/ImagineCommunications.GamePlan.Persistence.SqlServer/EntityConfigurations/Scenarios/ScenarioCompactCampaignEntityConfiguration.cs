using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios
{
    public class ScenarioCompactCampaignEntityConfiguration : IEntityTypeConfiguration<ScenarioCompactCampaign>
    {
        public void Configure(EntityTypeBuilder<ScenarioCompactCampaign> builder)
        {
            builder.ToTable("ScenarioCompactCampaigns");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.ActualRatings).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.TargetRatings).HasColumnType("DECIMAL(28,18)");

            builder.Property(e => e.Status).HasMaxLength(256);
            builder.Property(e => e.Name).HasMaxLength(256);
            builder.Property(e => e.ExternalId).HasMaxLength(256);
            builder.Property(e => e.CampaignGroup).HasMaxLength(256);
            builder.Property(e => e.ProductExternalRef).HasMaxLength(256);
            builder.Property(e => e.ProductName).HasMaxLength(256);
            builder.Property(e => e.AdvertiserName).HasMaxLength(256);
            builder.Property(e => e.AgencyName).HasMaxLength(256);
            builder.Property(e => e.BusinessType).HasMaxLength(256);
            builder.Property(e => e.Demographic).HasMaxLength(256);
            builder.Property(e => e.ClashCode).HasMaxLength(256);
            builder.Property(e => e.ClashDescription).HasMaxLength(256);
            builder.Property(e => e.StartDateTime).AsUtc();
            builder.Property(e => e.EndDateTime).AsUtc();
            builder.Property(p => p.AgencyGroupShortName).HasMaxLength(256);
            builder.Property(e => e.AgencyGroupCode).HasMaxLength(256);
            builder.Property(e => e.ReportingCategory).HasMaxLength(256);
            builder.Property(p => p.SalesExecutiveName).HasMaxLength(256);
            builder.Property(p => p.CreationDate).AsUtc();
            builder.Property(e => e.ActiveLength).HasColumnType("NVARCHAR(MAX)");
            builder.Property(e => e.AchievedPercentageRevenueBudget).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.AchievedPercentageTargetRatings).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.RatingsDifferenceExcludingPayback).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.ValueDifference).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.ValueDifferenceExcludingPayback).HasColumnType("DECIMAL(28,18)");

            builder.HasMany(e => e.CampaignPaybacks)
                .WithOne()
                .HasForeignKey(e => e.ScenarioCompactCampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.ScenarioCampaignPassPriorityId);
        }
    }
}
