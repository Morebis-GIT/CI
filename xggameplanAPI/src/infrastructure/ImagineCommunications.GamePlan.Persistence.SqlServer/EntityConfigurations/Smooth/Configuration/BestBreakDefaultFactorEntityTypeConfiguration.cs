using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class BestBreakDefaultFactorEntityTypeConfiguration: IEntityTypeConfiguration<BestBreakDefaultFactor>
    {
        public void Configure(EntityTypeBuilder<BestBreakDefaultFactor> builder)
        {
            builder.ToTable("SmoothConfigurationBestBreakFactorGroupItemDefaultFactors");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();
        }
    }
}
