using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios
{
    public class ScenarioPassReferenceEntityConfiguration : IEntityTypeConfiguration<ScenarioPassReference>
    {
        public void Configure(EntityTypeBuilder<ScenarioPassReference> builder)
        {
            builder.ToTable("ScenarioPassReferences");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(x => x.Order).IsRequired().HasDefaultValue(1);

            builder.HasIndex(e => e.PassId);
            builder.HasIndex(e => e.ScenarioId);
            builder.HasIndex(e => e.Order);
        }
    }
}
