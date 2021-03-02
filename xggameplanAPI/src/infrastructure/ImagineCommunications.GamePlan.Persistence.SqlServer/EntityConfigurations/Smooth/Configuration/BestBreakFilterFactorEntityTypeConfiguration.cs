using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class BestBreakFilterFactorEntityTypeConfiguration: IEntityTypeConfiguration<BestBreakFilterFactor>
    {
        public void Configure(EntityTypeBuilder<BestBreakFilterFactor> builder)
        {
            builder.ToTable("SmoothConfigurationBestBreakFactorGroupItemFilterFactors");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();
        }
    }
}
