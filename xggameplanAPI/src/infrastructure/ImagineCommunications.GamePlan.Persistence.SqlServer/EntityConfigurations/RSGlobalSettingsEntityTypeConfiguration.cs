using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class RSGlobalSettingsEntityTypeConfiguration: IEntityTypeConfiguration<RSGlobalSettings>
    {
        public void Configure(EntityTypeBuilder<RSGlobalSettings> builder)
        {
            builder.ToTable("RSGlobalSettings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseSqlServerIdentityColumn();
        }
    }
}
