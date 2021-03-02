using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class RuleTypeEntityConfiguration : IEntityTypeConfiguration<RuleType>
    {
        public void Configure(EntityTypeBuilder<RuleType> builder)
        {
            builder.ToTable("RuleTypes");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
