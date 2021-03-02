using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class UniverseEntityConfiguration : IEntityTypeConfiguration<Universe>
    {
        public void Configure(EntityTypeBuilder<Universe> builder)
        {
            builder.ToTable("Universes");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id);

            builder.Property(p => p.SalesArea).HasMaxLength(64);
            builder.Property(p => p.Demographic).HasMaxLength(64);
            builder.Property(p => p.StartDate).AsUtc();
            builder.Property(p => p.EndDate).AsUtc();
            builder.Property(p => p.UniverseValue).IsRequired();

            builder.HasIndex(p => p.SalesArea);
            builder.HasIndex(p => p.Demographic);
            builder.HasIndex(p => p.StartDate);
            builder.HasIndex(p => p.EndDate);
        }
    }
}
