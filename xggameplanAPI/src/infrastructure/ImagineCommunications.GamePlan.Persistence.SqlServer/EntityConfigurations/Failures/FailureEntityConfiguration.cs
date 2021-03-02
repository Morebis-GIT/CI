using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Failures
{
    public class FailureEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.Failures.Failure>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.Failures.Failure> builder)
        {
            builder.ToTable("Failures");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(e => e.ScenarioId)
                .IsUnique();

            builder.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(p => p.FailureId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
