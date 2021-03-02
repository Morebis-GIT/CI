using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgStrikeWeightLengthEntityConfiguration : IEntityTypeConfiguration<AgStrikeWeightLength>
    {
        public void Configure(EntityTypeBuilder<AgStrikeWeightLength> builder)
        {
            builder.ToTable("AgStrikeWeightLengths");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.StartDate).HasMaxLength(32);
            builder.Property(e => e.EndDate).HasMaxLength(32);

            builder.HasIndex(e => e.AgStrikeWeightId);
        }
    }
}
