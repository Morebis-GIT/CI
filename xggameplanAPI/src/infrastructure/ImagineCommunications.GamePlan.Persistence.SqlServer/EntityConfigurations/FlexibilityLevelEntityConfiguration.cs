using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class FlexibilityLevelEntityConfiguration : IEntityTypeConfiguration<FlexibilityLevel>
    {
        public void Configure(EntityTypeBuilder<FlexibilityLevel> builder)
        {
            builder.ToTable("FlexibilityLevels");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Name).HasMaxLength(255).IsRequired();

            builder.HasIndex(e => e.Name).IsUnique();
        }
    }
}
