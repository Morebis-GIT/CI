using System;
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
            _ = builder.ToTable("Campaigns");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id);

            builder.Property(e => e.CustomId).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            builder.Property(e => e.ExternalId).HasMaxLength(64);
            builder.Property(e => e.Name).HasMaxLength(256);
            builder.Property(e => e.Demographic).HasMaxLength(64);
            builder.Property(e => e.StartDateTime).AsUtc();
            builder.Property(e => e.EndDateTime).AsUtc();
            builder.Property(e => e.Product).HasMaxLength(64);
            builder.Property(e => e.ActualRatings).AsDecimal(28, 18);
            builder.Property(e => e.TargetRatings).AsDecimal(28, 18);
            builder.Property(e => e.AchievedPercentageRevenueBudget).AsDecimal(28, 18);
            builder.Property(e => e.AchievedPercentageTargetRatings).AsDecimal(28, 18);
            builder.Property(e => e.RatingsDifferenceExcludingPayback).AsDecimal(28, 18);
            builder.Property(e => e.ValueDifferenceExcludingPayback).AsMoney();
            builder.Property(e => e.ValueDifference).AsMoney();
            builder.Property(e => e.RevenueBooked).AsMoney();
            builder.Property(e => e.RevenueBudget).IsRequired().HasDefaultValue(0).AsMoney();
            builder.HasFtsField(Campaign.SearchTokensFieldName,new string[]{ nameof(Campaign.Name), nameof(CampaignGroup), nameof(ExternalId), nameof(Campaign.BusinessType) });

            builder.Property(e => e.CampaignGroup).HasMaxLength(32);
            builder.Property(e => e.BusinessType).HasMaxLength(32);
            builder.Property(e => e.ActiveLength).HasMaxLength(Int32.MaxValue); 
            builder.Property(e => e.ExpectedClearanceCode).HasMaxLength(64);
            builder.Property(e => e.CreationDate).AsUtc();
            
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
