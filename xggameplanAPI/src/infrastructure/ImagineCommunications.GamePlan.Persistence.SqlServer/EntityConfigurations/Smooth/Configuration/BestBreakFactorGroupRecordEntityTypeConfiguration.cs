using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class BestBreakFactorGroupRecordEntityTypeConfiguration: IEntityTypeConfiguration<BestBreakFactorGroupRecord>
    {
        public void Configure(EntityTypeBuilder<BestBreakFactorGroupRecord> builder)
        {
            builder.ToTable("SmoothConfigurationBestBreakFactorGroupRecords");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();

            builder.Property(x => x.SpotsCriteriaHasSponsoredSpots);
            builder.Property(x => x.SpotsCriteriaHasFIBORLIBRequests);
            builder.Property(x => x.SpotsCriteriaHasBreakRequest);

            builder.HasOne(x => x.BestBreakFactorGroup).WithOne()
                .HasForeignKey<BestBreakFactorGroup>(x => x.BestBreakFactorGroupRecordId);

            builder.HasMany(x => x.PassSequences).WithOne().HasForeignKey(x => x.BestBreakFactorGroupRecordId);
        }
    }
}
