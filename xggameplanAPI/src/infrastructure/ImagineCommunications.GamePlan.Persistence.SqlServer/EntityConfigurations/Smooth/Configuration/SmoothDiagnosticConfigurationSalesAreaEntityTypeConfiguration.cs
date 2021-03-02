using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SmoothDiagnosticConfigurationSalesAreaEntityTypeConfiguration : IEntityTypeConfiguration<SmoothDiagnosticConfigurationSalesArea>
    {
        public void Configure(EntityTypeBuilder<SmoothDiagnosticConfigurationSalesArea> builder)
        {
            _ = builder.ToTable("SmoothDiagnosticConfigurationSalesAreas");
            _ = builder.HasKey(x => x.Id);
            _ = builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            _ = builder.HasIndex(x => x.SmoothDiagnosticConfigurationId);
            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
