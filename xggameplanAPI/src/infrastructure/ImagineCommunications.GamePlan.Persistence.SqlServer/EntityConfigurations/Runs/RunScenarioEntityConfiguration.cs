using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunScenarioEntityConfiguration : IEntityTypeConfiguration<RunScenario>
    {
        public void Configure(EntityTypeBuilder<RunScenario> builder)
        {
            builder.ToTable("RunScenarios");

            builder.HasKey(k => k.Id);

            builder.Property(p => p.Id).UseSqlServerIdentityColumn();

            builder.Property(p => p.StartedDateTime).AsUtc();
            builder.Property(p => p.CompletedDateTime).AsUtc();
            builder.Property(p => p.Progress).HasMaxLength(32);
            builder.Property(x => x.Order).IsRequired().HasDefaultValue(1);
            builder.HasIndex(e => e.RunId);
            builder.HasIndex(e => e.ScenarioId);
            builder.HasIndex(e => e.Order);
        }
    }
}
