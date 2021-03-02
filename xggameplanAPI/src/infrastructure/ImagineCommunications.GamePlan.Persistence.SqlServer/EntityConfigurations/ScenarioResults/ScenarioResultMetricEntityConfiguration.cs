using ImagineCommunications.GamePlan.Domain;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ScenarioResults
{
    public class ScenarioResultMetricEntityConfiguration : IEntityTypeConfiguration<ScenarioResultMetric>
    {
        public void Configure(EntityTypeBuilder<ScenarioResultMetric> builder)
        {
            builder.ToTable("ScenarioResultMetrics");
            builder.HasKey(e => e.Id);
            builder.HasDiscriminator(c => c.ResultSource)
                .HasValue<GameplanScenarioResultMetric>(KPISource.Gameplan)
                .HasValue<LandmarkScenarioResultMetric>(Domain.KPISource.Landmark);

            
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();
            builder.Property(e => e.DisplayFormat).HasMaxLength(32).IsRequired();
            builder.Property(e => e.ResultSource).HasDefaultValue(KPISource.Gameplan);
        }
    }
}
