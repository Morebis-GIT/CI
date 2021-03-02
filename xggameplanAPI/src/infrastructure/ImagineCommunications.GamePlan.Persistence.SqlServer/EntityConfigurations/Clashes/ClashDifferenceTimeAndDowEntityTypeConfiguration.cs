using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ClashDifferenceTimeAndDowEntityTypeConfiguration : IEntityTypeConfiguration<ClashDifferenceTimeAndDow>
    {
        public void Configure(EntityTypeBuilder<ClashDifferenceTimeAndDow> builder)
        {
            builder.ToTable("ClashDifferenceTimeAndDows");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();

            builder.Property(x => x.StartTime).AsTicks();
            builder.Property(x => x.EndTime).AsTicks();
            builder.Property(x => x.DaysOfWeek).AsStringPattern(DayOfWeek.Monday).IsRequired();

            builder.HasIndex(x => x.ClashDifferenceId);
        }
    }
}
