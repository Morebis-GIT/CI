using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassBreakExclusionEntityConfiguration : IEntityTypeConfiguration<PassBreakExclusion>
    {
        public void Configure(EntityTypeBuilder<PassBreakExclusion> builder)
        {
            _ = builder.ToTable("PassBreakExclusions");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.Property(e => e.StartDate).AsUtc();
            _ = builder.Property(e => e.EndDate).AsUtc();
            _ = builder.Property(e => e.StartTime).AsTicks();
            _ = builder.Property(e => e.EndTime).AsTicks();
            _ = builder.Property(e => e.SelectableDays).AsStringPattern(DayOfWeek.Sunday);

            _ = builder.HasOne(x => x.SalesArea)
                .WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(x => x.PassId);
            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
