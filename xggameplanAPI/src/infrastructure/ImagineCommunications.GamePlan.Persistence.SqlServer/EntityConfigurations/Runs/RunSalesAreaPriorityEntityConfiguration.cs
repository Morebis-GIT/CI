using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunSalesAreaPriorityEntityConfiguration : IEntityTypeConfiguration<RunSalesAreaPriority>
    {
        public void Configure(EntityTypeBuilder<RunSalesAreaPriority> builder)
        {
            _ = builder.ToTable("RunSalesAreaPriorities");

            _ = builder.HasKey(k => k.Id);
            _ = builder.Property(p => p.Id).UseSqlServerIdentityColumn();
            _ = builder.HasOne(x => x.SalesArea)
                .WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(e => e.RunId);
            _ = builder.HasIndex(e => e.SalesAreaId);
        }
    }
}
