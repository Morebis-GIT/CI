using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class DemographicEntityConfiguration : IEntityTypeConfiguration<Demographic>
    {
        public void Configure(EntityTypeBuilder<Demographic> builder)
        {
            builder.ToTable("Demographics");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.ExternalRef).HasMaxLength(64);
            builder.Property(p => p.Name).HasMaxLength(256);
            builder.Property(p => p.ShortName).HasMaxLength(128);

            builder.HasIndex(p => p.ExternalRef);
        }
    }
}
