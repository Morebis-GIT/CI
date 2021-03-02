using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ScenarioResults
{
    public class ScenarioResultEntityConfiguration : IEntityTypeConfiguration<ScenarioResult>
    {
        public void Configure(EntityTypeBuilder<ScenarioResult> builder)
        {
            builder.ToTable("ScenarioResults");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.TimeCompleted).AsUtc();

            builder.HasIndex(x => x.ScenarioId);

            builder.HasMany(e => e.LandmarkMetrics)
                .WithOne()
                .HasForeignKey(e => e.ScenarioResultId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ScenarioResultMetrics_ScenarioResults_ScenarioResultId");

            builder.HasMany(e => e.Metrics)
                .WithOne()
                .HasForeignKey(e => e.ScenarioResultId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ScenarioResultMetrics_ScenarioResults_ScenarioResultId");

            builder.HasMany(e => e.AnalysisGroupMetrics)
                .WithOne()
                .HasForeignKey(e => e.ScenarioResultId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
