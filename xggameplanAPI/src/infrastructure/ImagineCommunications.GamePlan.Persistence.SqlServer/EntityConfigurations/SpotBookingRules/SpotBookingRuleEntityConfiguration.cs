using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SpotBookingRules
{
    public class SpotBookingRuleEntityConfiguration : IEntityTypeConfiguration<SpotBookingRule>
    {
        public void Configure(EntityTypeBuilder<SpotBookingRule> builder)
        {
            builder.ToTable("SpotBookingRules");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.SpotLength).AsTicks();
            builder.Property(e => e.MinBreakLength).AsTicks();
            builder.Property(e => e.MaxBreakLength).AsTicks();
            builder.Property(e => e.BreakType).HasMaxLength(32);

            builder.HasMany(x => x.SalesAreas).WithOne().HasForeignKey(x => x.SpotBookingRuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
