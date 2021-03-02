using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunSalesAreaPriorityEntityConfiguration : IEntityTypeConfiguration<RunSalesAreaPriority>
    {
        public void Configure(EntityTypeBuilder<RunSalesAreaPriority> builder)
        {
            builder.ToTable("RunSalesAreaPriorities");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.SalesArea).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.RunId);
            builder.HasIndex(e => e.SalesArea);
        }
    }
}
