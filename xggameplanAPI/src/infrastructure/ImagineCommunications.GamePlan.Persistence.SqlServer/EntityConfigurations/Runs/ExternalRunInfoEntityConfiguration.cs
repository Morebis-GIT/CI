using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class ExternalRunInfoEntityConfiguration : IEntityTypeConfiguration<ExternalRunInfo>
    {
        public void Configure(EntityTypeBuilder<ExternalRunInfo> builder)
        {
            builder.ToTable("ExternalRunInfo");

            builder.HasKey(x => x.RunScenarioId);

            builder.Property(p => p.ExternalStatusModifiedDate).AsUtc();
            builder.Property(p => p.ScheduledDateTime).AsUtc();
            builder.Property(p => p.CreatedDateTime).AsUtc();

            builder.HasOne<RunScenario>().WithOne(x => x.ExternalRunInfo)
                .HasForeignKey<ExternalRunInfo>(x => x.RunScenarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
