using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassRatingPointEntityConfiguration : IEntityTypeConfiguration<PassRatingPoint>
    {
        public void Configure(EntityTypeBuilder<PassRatingPoint> builder)
        {
            builder.ToTable("PassRatingPoints");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.SalesAreas).AsDelimitedString();

            builder.HasIndex(e => e.PassId);
        }
    }
}
