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
            _ = builder.ToTable("Universes");

            _ = builder.HasKey(k => k.Id);
            _ = builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");

            _ = builder.Property(p => p.Demographic).HasMaxLength(64);
            _ = builder.Property(p => p.StartDate).AsUtc();
            _ = builder.Property(p => p.EndDate).AsUtc();
            _ = builder.Property(p => p.UniverseValue).IsRequired();

            _ = builder.HasOne(x => x.SalesArea).WithMany().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(p => p.SalesAreaId);
            _ = builder.HasIndex(p => p.Demographic);
            _ = builder.HasIndex(p => p.StartDate);
            _ = builder.HasIndex(p => p.EndDate);
        }
    }
}
