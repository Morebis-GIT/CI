using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class AutopilotRuleEntityConfiguration : IEntityTypeConfiguration<AutopilotRule>
    {
        public void Configure(EntityTypeBuilder<AutopilotRule> builder)
        {
            builder.ToTable("AutopilotRules");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();
        }
    }
}
