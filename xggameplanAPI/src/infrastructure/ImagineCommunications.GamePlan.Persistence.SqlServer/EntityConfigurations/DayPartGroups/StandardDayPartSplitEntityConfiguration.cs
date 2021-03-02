using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.DayPartGroups
{
    public class StandardDayPartSplitEntityConfiguration : IEntityTypeConfiguration<StandardDayPartSplit>
    {
        public void Configure(EntityTypeBuilder<StandardDayPartSplit> builder)
        {
            builder.ToTable("StandardDayPartSplits");
            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
        }
    }
}
