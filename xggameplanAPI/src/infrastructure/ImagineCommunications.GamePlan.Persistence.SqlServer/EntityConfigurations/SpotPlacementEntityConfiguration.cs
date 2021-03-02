using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class SpotPlacementEntityConfiguration : IEntityTypeConfiguration<SpotPlacement>
    {
        public void Configure(EntityTypeBuilder<SpotPlacement> builder)
        {
            builder.ToTable("SpotPlacements");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.ModifiedTime).AsUtc().IsRequired();
            builder.Property(p => p.ExternalSpotRef).HasMaxLength(64);
            builder.Property(p => p.ExternalBreakRef).HasMaxLength(64);
            builder.Property(p => p.ResetExternalBreakRef).HasMaxLength(64);

            builder.HasIndex(e => e.ExternalSpotRef).IsUnique();
            builder.HasIndex(e => e.ModifiedTime);
            builder.HasIndex(e => e.ExternalBreakRef);
        }
    }
}
