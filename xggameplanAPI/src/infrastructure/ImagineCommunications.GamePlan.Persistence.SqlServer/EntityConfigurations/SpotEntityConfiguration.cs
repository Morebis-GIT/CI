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
            _ = builder.ToTable("Spots");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.Property(e => e.Uid).HasDefaultValueSql("newid()");
            _ = builder.Property(e => e.SpotLength).AsTicks();
            _ = builder.Property(e => e.ExternalCampaignNumber).HasMaxLength(64);
            _ = builder.Property(e => e.GroupCode).HasMaxLength(32);
            _ = builder.Property(e => e.ExternalSpotRef).HasMaxLength(64);
            _ = builder.Property(e => e.BreakType).HasMaxLength(32);
            _ = builder.Property(e => e.Product).HasMaxLength(64);
            _ = builder.Property(e => e.Demographic).HasMaxLength(64);
            _ = builder.Property(e => e.MultipartSpot).HasMaxLength(16);
            _ = builder.Property(e => e.MultipartSpotPosition).HasMaxLength(16);
            _ = builder.Property(e => e.RequestedPositioninBreak).HasMaxLength(16);
            _ = builder.Property(e => e.ActualPositioninBreak).HasMaxLength(16);
            _ = builder.Property(e => e.BreakRequest).HasMaxLength(32);
            _ = builder.Property(e => e.ExternalBreakNo).HasMaxLength(64);
            _ = builder.Property(e => e.IndustryCode).HasMaxLength(32);
            _ = builder.Property(e => e.ClearanceCode).HasMaxLength(64);
            _ = builder.Property(e => e.NominalPrice).HasColumnType("DECIMAL(28,18)");
            _ = builder.Property(e => e.StartDateTime).AsUtc();
            _ = builder.Property(e => e.EndDateTime).AsUtc();

            _ = builder.HasOne(x => x.SalesArea)
                .WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(e => e.Uid).IsUnique();
            _ = builder.HasIndex(e => e.ExternalCampaignNumber);
            _ = builder.HasIndex(e => e.SalesAreaId);
            _ = builder.HasIndex(e => e.ExternalSpotRef);
            _ = builder.HasIndex(e => e.ExternalBreakNo);
            _ = builder.HasIndex(e => e.MultipartSpot);
            _ = builder.HasIndex(e => e.StartDateTime);
        }
    }
}
