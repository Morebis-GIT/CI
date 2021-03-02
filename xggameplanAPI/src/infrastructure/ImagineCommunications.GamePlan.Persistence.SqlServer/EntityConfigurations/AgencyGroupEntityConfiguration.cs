using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class AgencyGroupEntityConfiguration : IEntityTypeConfiguration<AgencyGroup>
    {
        public void Configure(EntityTypeBuilder<AgencyGroup> builder)
        {
            builder.ToTable("AgencyGroups");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.ShortName).HasMaxLength(128);
            builder.Property(p => p.Code).HasMaxLength(128);

            builder.HasIndex(p => new { p.ShortName, p.Code }).IsUnique();
        }
    }
}
