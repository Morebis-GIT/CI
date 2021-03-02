using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BRS
{
    public class BRSConfigurationTemplateEntityConfiguration : IEntityTypeConfiguration<BRSConfigurationTemplate>
    {
        public void Configure(EntityTypeBuilder<BRSConfigurationTemplate> builder)
        {
            builder.ToTable("BRSConfigurationTemplates");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);

            builder.HasMany(e => e.KPIConfigurations).WithOne()
                .HasForeignKey(e => e.BRSConfigurationTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
