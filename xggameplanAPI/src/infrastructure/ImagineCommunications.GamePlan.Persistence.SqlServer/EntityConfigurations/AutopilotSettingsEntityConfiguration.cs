using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class AutopilotSettingsEntityConfiguration : IEntityTypeConfiguration<AutopilotSettings>
    {
        public void Configure(EntityTypeBuilder<AutopilotSettings> builder)
        {
            builder.ToTable("AutopilotSettings");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();
        }
    }
}
