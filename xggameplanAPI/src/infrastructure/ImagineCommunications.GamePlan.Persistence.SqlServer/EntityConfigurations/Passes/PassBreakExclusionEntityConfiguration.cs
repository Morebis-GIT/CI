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
            builder.ToTable("PassBreakExclusions");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.SalesArea).IsRequired(true).HasMaxLength(64);
            builder.Property(e => e.StartDate).AsUtc();
            builder.Property(e => e.EndDate).AsUtc();
            builder.Property(e => e.StartTime).AsTicks();
            builder.Property(e => e.EndTime).AsTicks();
            builder.Property(e => e.SelectableDays).AsStringPattern(DayOfWeek.Sunday);

            builder.HasIndex(x => x.PassId);
        }
    }
}
