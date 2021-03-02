using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AnalysisGroups;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AnalysisGroups
{
    public class AnalysisGroupEntityConfiguration : AuditEntityTypeConfiguration<AnalysisGroup>
    {
        public override void Configure(EntityTypeBuilder<AnalysisGroup> builder)
        {
            builder.ToTable("AnalysisGroups");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
            builder.Property(x => x.CreatedBy).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            builder.Property(x => x.FilterAdvertiserExternalIds).AsDelimitedString();
            builder.Property(x => x.FilterAgencyExternalIds).AsDelimitedString();
            builder.Property(x => x.FilterAgencyGroupCodes).AsDelimitedString();
            builder.Property(x => x.FilterCampaignExternalIds).AsDelimitedString();
            builder.Property(x => x.FilterClashExternalRefs).AsDelimitedString();
            builder.Property(x => x.FilterBusinessTypes).AsDelimitedString();
            builder.Property(x => x.FilterProductExternalIds).AsDelimitedString();
            builder.Property(x => x.FilterReportingCategories).AsDelimitedString();
            builder.Property(x => x.FilterSalesExecExternalIds).AsDelimitedString();

            builder.HasIndex(x => x.IsDeleted);

            // do not query deactivated entries
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
