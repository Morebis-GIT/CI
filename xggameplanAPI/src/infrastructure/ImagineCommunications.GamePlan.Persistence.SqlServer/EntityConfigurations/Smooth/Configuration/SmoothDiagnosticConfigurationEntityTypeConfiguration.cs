using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SmoothDiagnosticConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<SmoothDiagnosticConfiguration>
    {
        public void Configure(EntityTypeBuilder<SmoothDiagnosticConfiguration> builder)
        {
            _ = builder.ToTable("SmoothDiagnosticConfigurations");
            _ = builder.HasKey(x => x.Id);

            _ = builder.Property(x => x.Id).UseSqlServerIdentityColumn();


            _ = builder.Property(x => x.SpotDemographics).AsDelimitedString().IsRequired();
            _ = builder.Property(x => x.SpotExternalRefs).AsDelimitedString().IsRequired();
            _ = builder.Property(x => x.SpotExternalCampaignRefs).AsDelimitedString().IsRequired();
            _ = builder.Property(x => x.SpotMultipartSpots).AsDelimitedString().IsRequired();

            _ = builder.Property(x => x.SpotMinStartTime).AsUtc();
            _ = builder.Property(x => x.SpotMaxStartTime).AsUtc();

            _ = builder.HasMany(x => x.SpotSalesAreas).WithOne(x => x.SmoothDiagnosticConfiguration).IsRequired();
            
        }
    }
}
