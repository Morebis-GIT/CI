using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SpotBookingRules
{
    public class SpotBookingRuleSalesAreaEntityConfiguration : IEntityTypeConfiguration<SpotBookingRuleSalesArea>
    {
        public void Configure(EntityTypeBuilder<SpotBookingRuleSalesArea> builder)
        {
            builder.ToTable("SpotBookingRuleSalesAreas");
            builder.HasKey(e => e.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.Name).HasMaxLength(512).IsRequired();
            builder.HasIndex(e => e.SpotBookingRuleId);
        }
    }
}
