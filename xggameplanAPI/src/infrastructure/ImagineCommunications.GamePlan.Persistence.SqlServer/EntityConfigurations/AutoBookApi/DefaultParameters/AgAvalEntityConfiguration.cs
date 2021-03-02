using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgAvalEntityConfiguration : IEntityTypeConfiguration<AgAval>
    {
        public void Configure(EntityTypeBuilder<AgAval> builder)
        {
            builder.ToTable("AgAvals");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(e => e.AutoBookDefaultParametersId);
        }
    }
}
