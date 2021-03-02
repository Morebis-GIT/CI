using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.InventoryStatuses
{
    public class LockTypeItemEntityConfiguration : IEntityTypeConfiguration<InventoryLockType>
    {
        public void Configure(EntityTypeBuilder<InventoryLockType> builder)
        {
            builder.ToTable("LockTypes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
        }
    }
}
