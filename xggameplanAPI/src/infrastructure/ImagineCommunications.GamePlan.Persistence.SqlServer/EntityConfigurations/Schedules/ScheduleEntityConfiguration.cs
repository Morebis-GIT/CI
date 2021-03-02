using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Schedules
{
    public class ScheduleEntityConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            _ = builder.ToTable("Schedules");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.Property(p => p.Date).AsUtc();
            _ = builder.Ignore(e => e.ScheduleUniqueKey);

            _ = builder.HasIndex(e => e.SalesAreaId);
            _ = builder.HasIndex(e => new { e.Date, e.SalesAreaId }).IsUnique();

            _ = builder.HasMany(e => e.Breaks).WithOne().HasForeignKey(e => e.ScheduleId);
            _ = builder.HasMany(e => e.Programmes).WithOne().HasForeignKey(e => e.ScheduleId);
            _ = builder.HasOne(e => e.SalesArea).WithMany().HasForeignKey(e => e.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
