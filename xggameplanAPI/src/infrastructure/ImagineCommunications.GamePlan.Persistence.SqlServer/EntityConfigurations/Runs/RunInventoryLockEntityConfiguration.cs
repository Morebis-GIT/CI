using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunInventoryLockEntityConfiguration : IEntityTypeConfiguration<RunInventoryLock>
    {
        public void Configure(EntityTypeBuilder<RunInventoryLock> builder)
        {
            builder.ToTable("RunInventoryLocks");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();

            builder.HasIndex(e => e.RunId);
            builder.HasIndex(e => e.ChosenScenarioId);
        }
    }
}
