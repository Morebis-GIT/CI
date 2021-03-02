using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.InventoryStatuses
{
    public class InventoryLockEntityConfiguration : IEntityTypeConfiguration<InventoryLock>
    {
        public void Configure(EntityTypeBuilder<InventoryLock> builder)
        {
            _ = builder.ToTable("InventoryLocks");
            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseMySqlIdentityColumn();
            _ = builder.Property(e => e.InventoryCode).HasColumnType("NCHAR(10)").IsRequired();
            _ = builder.Property(e => e.StartDate).AsUtc();
            _ = builder.Property(e => e.EndDate).AsUtc();
            _ = builder.Property(e => e.StartTime).AsTicks();
            _ = builder.Property(e => e.EndTime).AsTicks();

            _ = builder.HasOne(x => x.SalesArea).WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
