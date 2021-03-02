using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.InventoryStatuses
{
    public class RunInventoryStatusEntityConfiguration : IEntityTypeConfiguration<RunInventoryStatus>
    {
        public void Configure(EntityTypeBuilder<RunInventoryStatus> builder)
        {
            builder.ToTable("RunExcludedInventoryStatuses");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.RunId);
            builder.Property(e => e.InventoryCode).HasMaxLength(10).IsRequired();

            builder.HasOne(e => e.Run)
                .WithMany(e => e.ExcludedInventoryStatuses)
                .HasForeignKey(e => e.RunId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.RunId);
        }
    }
}
