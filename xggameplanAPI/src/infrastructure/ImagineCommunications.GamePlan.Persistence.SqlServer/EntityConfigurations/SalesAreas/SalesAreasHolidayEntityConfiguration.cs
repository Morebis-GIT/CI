using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SalesAreas
{
    public class SalesAreasHolidayEntityConfiguration : IEntityTypeConfiguration<SalesAreasHoliday>
    {
        public void Configure(EntityTypeBuilder<SalesAreasHoliday> builder)
        {
            builder.ToTable("SalesAreasHolidays");
            builder.HasKey(e => e.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.Type).IsRequired();
            builder.Property(e => e.Start).IsRequired();
            builder.Property(e => e.End).IsRequired();
            builder.Property(e => e.Start).AsUtc();
            builder.Property(e => e.End).AsUtc();
            builder.HasIndex(e => e.SalesAreaId);
        }
    }
}
