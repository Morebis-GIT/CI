using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.InventoryStatuses
{
    public class InventoryTypeEntityConfiguration : IEntityTypeConfiguration<InventoryType>
    {
        public void Configure(EntityTypeBuilder<InventoryType> builder)
        {
            builder.ToTable("InventoryTypes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.InventoryCode).HasColumnType("CHAR(10)").IsRequired();
            builder.Property(e => e.Description).HasMaxLength(50).IsRequired();
            builder.Property(e => e.System).HasColumnType("CHAR(10)").IsRequired();

            builder.HasMany(x => x.LockTypes).WithOne()
                .HasForeignKey(x => x.InventoryTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
