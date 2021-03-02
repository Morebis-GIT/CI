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
            builder.ToTable("SmoothDiagnosticConfigurations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseMySqlIdentityColumn();

            builder.Property(x => x.SpotSalesAreas).AsDelimitedString().IsRequired();
            builder.Property(x => x.SpotDemographics).AsDelimitedString().IsRequired();
            builder.Property(x => x.SpotExternalRefs).AsDelimitedString().IsRequired();
            builder.Property(x => x.SpotExternalCampaignRefs).AsDelimitedString().IsRequired();
            builder.Property(x => x.SpotMultipartSpots).AsDelimitedString().IsRequired();

            builder.Property(x => x.SpotMinStartTime).AsUtc();
            builder.Property(x => x.SpotMaxStartTime).AsUtc();
        }
    }
}
