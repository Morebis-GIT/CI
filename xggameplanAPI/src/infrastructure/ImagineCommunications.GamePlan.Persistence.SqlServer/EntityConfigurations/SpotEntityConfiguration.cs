using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class SpotEntityConfiguration : IEntityTypeConfiguration<Spot>
    {
        public void Configure(EntityTypeBuilder<Spot> builder)
        {
            builder.ToTable("Spots");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Uid);
            builder.Property(e => e.SpotLength).AsTicks();
            builder.Property(e => e.ExternalCampaignNumber).HasMaxLength(64);
            builder.Property(e => e.SalesArea).HasMaxLength(64);
            builder.Property(e => e.GroupCode).HasMaxLength(32);
            builder.Property(e => e.ExternalSpotRef).HasMaxLength(64);
            builder.Property(e => e.BreakType).HasMaxLength(32);
            builder.Property(e => e.Product).HasMaxLength(64);
            builder.Property(e => e.Demographic).HasMaxLength(64);
            builder.Property(e => e.MultipartSpot).HasMaxLength(16);
            builder.Property(e => e.MultipartSpotPosition).HasMaxLength(16);
            builder.Property(e => e.RequestedPositioninBreak).HasMaxLength(16);
            builder.Property(e => e.ActualPositioninBreak).HasMaxLength(16);
            builder.Property(e => e.BreakRequest).HasMaxLength(32);
            builder.Property(e => e.ExternalBreakNo).HasMaxLength(64);
            builder.Property(e => e.IndustryCode).HasMaxLength(32);
            builder.Property(e => e.ClearanceCode).HasMaxLength(64);
            builder.Property(e => e.NominalPrice).HasColumnType("DECIMAL(28,18)");
            builder.Property(e => e.StartDateTime).AsUtc();
            builder.Property(e => e.EndDateTime).AsUtc();

            builder.HasIndex(e => e.Uid).IsUnique();
            builder.HasIndex(e => e.ExternalCampaignNumber);
            builder.HasIndex(e => e.SalesArea);
            builder.HasIndex(e => e.ExternalSpotRef);
            builder.HasIndex(e => e.ExternalBreakNo);
            builder.HasIndex(e => e.MultipartSpot);
            builder.HasIndex(e => e.StartDateTime);
        }
    }
}
