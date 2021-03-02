using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class BestBreakFactorGroupEntityTypeConfiguration: IEntityTypeConfiguration<BestBreakFactorGroup>
    {
        public void Configure(EntityTypeBuilder<BestBreakFactorGroup> builder)
        {
            builder.ToTable("SmoothConfigurationBestBreakFactorGroups");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.SameBreakGroupScoreFactor).WithOne()
                .HasForeignKey<SameBreakGroupScoreFactor>(x => x.BestBreakFactorGroupId);

            builder.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.BestBreakFactorGroupId);
        }
    }
}
