using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgPredictionEntityConfiguration : IEntityTypeConfiguration<AgPrediction>
    {
        public void Configure(EntityTypeBuilder<AgPrediction> builder)
        {
            builder.ToTable("AgPredictions");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.HasIndex(e => e.AutoBookDefaultParametersId);
        }
    }
}
