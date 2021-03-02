using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SmoothConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<SmoothConfiguration>
    {
        public void Configure(EntityTypeBuilder<SmoothConfiguration> builder)
        {
            RelationalEntityTypeBuilderExtensions.ToTable((EntityTypeBuilder) builder, "SmoothConfigurations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.ExternalCampaignRefsToExclude).AsDelimitedString().IsRequired();

            builder.HasOne(x => x.DiagnosticConfiguration).WithOne()
                .HasForeignKey<SmoothDiagnosticConfiguration>(x => x.SmoothConfigurationId);

            builder.HasMany(x => x.Passes).WithOne().HasForeignKey(x => x.SmoothConfigurationId);
            builder.HasMany(x => x.IterationRecords).WithOne().HasForeignKey(x => x.SmoothConfigurationId);
            builder.HasMany(x => x.BestBreakFactorGroupRecords).WithOne().HasForeignKey(x => x.SmoothConfigurationId);
        }
    }
}
