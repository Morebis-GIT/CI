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

            builder.Property(x => x.Id).UseMySqlIdentityColumn();

            builder.Property(x => x.SpotsCriteriaHasBreakRequest);
            builder.Property(x => x.SpotsCriteriaHasFIBORLIBRequests);
            builder.Property(x => x.SpotsCriteriaHasSponsoredSpots);

            builder.HasOne(x => x.PassDefaultIteration).WithOne()
                .HasForeignKey<SmoothPassDefaultIteration>(x => x.SmoothPassIterationRecordId);
            builder.HasOne(x => x.PassUnplacedIteration).WithOne()
                .HasForeignKey<SmoothPassUnplacedIteration>(x => x.SmoothPassIterationRecordId);

            builder.HasMany(x => x.PassSequences).WithOne().HasForeignKey(x => x.SmoothPassIterationRecordId);
        }
    }
}
