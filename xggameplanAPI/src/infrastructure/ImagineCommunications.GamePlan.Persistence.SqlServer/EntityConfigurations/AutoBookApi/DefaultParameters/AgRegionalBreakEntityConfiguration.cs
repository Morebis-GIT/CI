using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgRegionalBreakEntityConfiguration : IEntityTypeConfiguration<AgRegionalBreak>
    {
        public void Configure(EntityTypeBuilder<AgRegionalBreak> builder)
        {
            builder.ToTable("AgRegionalBreaks");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(e => e.AutoBookDefaultParametersId);
        }
    }
}
