using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SpotBookingRules
{
    public class SpotBookingRuleSalesAreaEntityConfiguration : IEntityTypeConfiguration<SpotBookingRuleSalesArea>
    {
        public void Configure(EntityTypeBuilder<SpotBookingRuleSalesArea> builder)
        {
            _ = builder.ToTable("SpotBookingRuleSalesAreas");
            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(p => p.Id).UseSqlServerIdentityColumn();
            _ = builder.Property(x => x.SalesAreaId);

            _ = builder.HasIndex(e => e.SpotBookingRuleId);
            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
