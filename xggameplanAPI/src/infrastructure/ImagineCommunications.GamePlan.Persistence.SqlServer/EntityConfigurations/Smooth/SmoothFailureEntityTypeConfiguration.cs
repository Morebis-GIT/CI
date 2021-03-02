using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth
{
    public class SmoothFailureEntityTypeConfiguration: IEntityTypeConfiguration<SmoothFailure>
    {
        public void Configure(EntityTypeBuilder<SmoothFailure> builder)
        {
            builder.ToTable("SmoothFailures");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.Property(x => x.BreakDateTime).AsUtc();
            builder.Property(x => x.SpotLength).AsTicks();
            builder.Property(x => x.RestrictionStartTime).AsTicks();
            builder.Property(x => x.RestrictionEndTime).AsTicks();
            builder.Property(x => x.RestrictionDays).AsStringPattern(DayOfWeek.Monday);

            builder.HasIndex(x => x.RunId);

            builder.HasMany(x => x.FailureMessagesMap).WithOne().HasForeignKey(x => x.SmoothFailureId);
        }
    }
}
