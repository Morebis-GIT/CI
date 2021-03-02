﻿using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
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
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.OwnsOne(x => x.SpotsCriteria, e =>
            {
                e.Property(x => x.HasSponsoredSpots).HasColumnName("SpotsCriteriaHasSponsoredSpots");
                e.Property(x => x.HasFIBORLIBRequests).HasColumnName("SpotsCriteriaHasFIBORLIBRequests");
                e.Property(x => x.HasBreakRequest).HasColumnName("SpotsCriteriaHasBreakRequest");
            });

            builder.HasOne(x => x.BestBreakFactorGroup).WithOne()
                .HasForeignKey<BestBreakFactorGroup>(x => x.BestBreakFactorGroupRecordId);

            builder.HasMany(x => x.PassSequences).WithOne().HasForeignKey(x => x.BestBreakFactorGroupRecordId);
        }
    }
}
