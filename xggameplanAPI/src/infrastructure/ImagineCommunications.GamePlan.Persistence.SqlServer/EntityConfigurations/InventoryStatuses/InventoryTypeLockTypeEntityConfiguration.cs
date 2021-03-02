using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.InventoryStatuses
{
    public class InventoryTypeLockTypeEntityConfiguration : IEntityTypeConfiguration<InventoryTypeLockType>
    {
        public void Configure(EntityTypeBuilder<InventoryTypeLockType> builder)
        {
            builder.ToTable("InventoryTypeLockTypes");
            builder.HasKey(e => e.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();

            builder.HasIndex(e => e.InventoryTypeId);
        }
    }
}
