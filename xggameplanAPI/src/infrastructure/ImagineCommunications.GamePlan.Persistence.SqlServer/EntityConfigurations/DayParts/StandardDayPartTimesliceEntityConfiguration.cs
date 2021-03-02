using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.DayParts
{
    public class StandardDayPartTimesliceEntityConfiguration : IEntityTypeConfiguration<StandardDayPartTimeslice>
    {
        public void Configure(EntityTypeBuilder<StandardDayPartTimeslice> builder)
        {
            builder.ToTable("StandardDayPartTimeslices");
            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.StartTime).AsTicks();
            builder.Property(e => e.EndTime).AsTicks();
        }
    }
}
