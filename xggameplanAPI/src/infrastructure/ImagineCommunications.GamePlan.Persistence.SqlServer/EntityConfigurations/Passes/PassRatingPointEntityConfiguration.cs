using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassRatingPointEntityConfiguration : IEntityTypeConfiguration<PassRatingPoint>
    {
        public void Configure(EntityTypeBuilder<PassRatingPoint> builder)
        {
            _ = builder.ToTable("PassRatingPoints");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.HasMany(e => e.SalesAreas)
                .WithOne()
                .HasForeignKey(e => e.PassRatingPointId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasIndex(e => e.PassId);
        }
    }
}
