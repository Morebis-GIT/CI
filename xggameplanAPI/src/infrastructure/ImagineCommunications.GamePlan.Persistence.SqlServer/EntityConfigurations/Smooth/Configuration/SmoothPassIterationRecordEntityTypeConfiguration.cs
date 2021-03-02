using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SmoothPassIterationRecordEntityTypeConfiguration : IEntityTypeConfiguration<SmoothPassIterationRecord>
    {
        public void Configure(EntityTypeBuilder<SmoothPassIterationRecord> builder)
        {
            builder.ToTable("SmoothConfigurationSmoothPassIterationRecords");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.OwnsOne(x => x.SpotsCriteria, e =>
            {
                e.Property(x => x.HasSponsoredSpots).HasColumnName("SpotsCriteriaHasSponsoredSpots");
                e.Property(x => x.HasFIBORLIBRequests).HasColumnName("SpotsCriteriaHasFIBORLIBRequests");
                e.Property(x => x.HasBreakRequest).HasColumnName("SpotsCriteriaHasBreakRequest");
            });

            builder.HasOne(x => x.PassDefaultIteration).WithOne()
                .HasForeignKey<SmoothPassDefaultIteration>(x => x.SmoothPassIterationRecordId);
            builder.HasOne(x => x.PassUnplacedIteration).WithOne()
                .HasForeignKey<SmoothPassUnplacedIteration>(x => x.SmoothPassIterationRecordId);

            builder.HasMany(x => x.PassSequences).WithOne().HasForeignKey(x => x.SmoothPassIterationRecordId);
        }
    }
}
