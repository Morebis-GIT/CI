using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ISRGlobalSettingsEntityTypeConfiguration : IEntityTypeConfiguration<ISRGlobalSettings>
    {
        public void Configure(EntityTypeBuilder<ISRGlobalSettings> builder)
        {
            builder.ToTable("ISRGlobalSettings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseMySqlIdentityColumn();
        }
    }
}
