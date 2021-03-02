using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SameBreakGroupScoreFactorEntityTypeConfiguration : IEntityTypeConfiguration<SameBreakGroupScoreFactor>
    {
        public void Configure(EntityTypeBuilder<SameBreakGroupScoreFactor> builder)
        {
            builder.ToTable("SmoothConfigurationSameBreakGroupScoreFactors");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseMySqlIdentityColumn();
        }
    }
}
