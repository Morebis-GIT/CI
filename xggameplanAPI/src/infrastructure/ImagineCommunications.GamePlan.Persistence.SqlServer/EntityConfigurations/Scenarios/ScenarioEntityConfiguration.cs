using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios
{
    public class ScenarioEntityConfiguration : IEntityTypeConfiguration<Scenario>
    {
        public void Configure(EntityTypeBuilder<Scenario> builder)
        {
            builder.ToTable("Scenarios");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("newid()");
            builder.Property(e => e.Name).HasMaxLength(256);
            builder.Property<string>(Scenario.SearchField)
                .HasComputedColumnSql("CONCAT_WS(' ', Id, [Name])");

            builder.HasIndex(e => e.IsLibraried);

            builder.HasOne(e => e.CampaignPriorityRounds).WithOne()
                .HasForeignKey<ScenarioCampaignPriorityRoundCollection>(e => e.ScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.PassReferences)
                .WithOne()
                .HasForeignKey(e => e.ScenarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.CampaignPassPriorities)
                .WithOne()
                .HasForeignKey(e => e.ScenarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
