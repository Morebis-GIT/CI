using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ScenarioResults
{
    public class AnalysisGroupTargetMetricEntityConfiguration : IEntityTypeConfiguration<AnalysisGroupTargetMetric>
    {
        public void Configure(EntityTypeBuilder<AnalysisGroupTargetMetric> builder)
        {
            builder.ToTable("AnalysisGroupTargetMetrics");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.HasIndex(x => new {x.ScenarioResultId, x.AnalysisGroupTargetId}).IsUnique();
        }
    }
}
