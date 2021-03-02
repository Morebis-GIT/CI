using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class BestBreakFactorGroupItemEntityTypeConfiguration: IEntityTypeConfiguration<BestBreakFactorGroupItem>
    {
        public void Configure(EntityTypeBuilder<BestBreakFactorGroupItem> builder)
        {
            builder.ToTable("SmoothConfigurationBestBreakFactorGroupItems");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();

            builder.HasMany(x => x.DefaultFactors).WithOne().HasForeignKey(x => x.BestBreakFactorGroupItemId);
            builder.HasMany(x => x.FilterFactors).WithOne().HasForeignKey(x => x.BestBreakFactorGroupItemId);
        }
    }
}
