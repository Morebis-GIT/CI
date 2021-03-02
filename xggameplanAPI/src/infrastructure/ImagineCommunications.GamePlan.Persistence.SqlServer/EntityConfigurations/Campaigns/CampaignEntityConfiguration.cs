using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns
{
    public class CampaignEntityConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("newid()");

            builder.Property(e => e.CustomId).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            builder.Property(e => e.ExternalId).HasMaxLength(64);
            builder.Property(e => e.Name).HasMaxLength(256);
            builder.Property(e => e.Demographic).HasMaxLength(64);
            builder.Property(e => e.StartDateTime).AsUtc();
            builder.Property(e => e.EndDateTime).AsUtc();
            builder.Property(e => e.Product).HasMaxLength(64);
            builder.Property(e => e.ActualRatings).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.TargetRatings).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.AchievedPercentageRevenueBudget).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.AchievedPercentageTargetRatings).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.RatingsDifferenceExcludingPayback).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.ValueDifferenceExcludingPayback).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.ValueDifference).HasColumnType("DECIMAL(28,18)");

            builder.Property(e => e.CampaignGroup).HasMaxLength(32);
            builder.Property(e => e.BusinessType).HasMaxLength(32);
            builder.Property(e => e.ActiveLength).HasColumnType("NVARCHAR(MAX)");
            builder.Property(e => e.ExpectedClearanceCode).HasMaxLength(64);
            builder.Property(e => e.CreationDate).AsUtc();
            builder.Property<string>(Campaign.SearchTokensFieldName)
                .HasComputedColumnSql("CONCAT_WS(' ',[CampaignGroup],[Name],[ExternalId],[BusinessType])");

            builder.HasIndex(e => e.CustomId).IsUnique();
            builder.HasIndex(e => e.ExternalId);
            builder.HasIndex(e => e.Product);
            builder.HasIndex(e => e.Demographic);
            builder.HasIndex(e => e.ExpectedClearanceCode);
            builder.HasIndex(e => e.CampaignGroup);

            builder.HasMany(x => x.BreakTypes).WithOne().HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.ProgrammeRestrictions).WithOne().HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.TimeRestrictions).WithOne().HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.SalesAreaCampaignTargets).WithOne().HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.BookingPositionGroups).WithOne().HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.CampaignPaybacks).WithOne().HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
