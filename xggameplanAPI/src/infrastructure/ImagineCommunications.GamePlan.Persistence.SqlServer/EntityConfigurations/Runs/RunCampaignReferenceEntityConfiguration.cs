using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunCampaignReferenceEntityConfiguration : IEntityTypeConfiguration<RunCampaignReference>
    {
        public void Configure(EntityTypeBuilder<RunCampaignReference> builder)
        {
            builder.ToTable("RunCampaignReferences");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();

            builder.Property(p => p.ExternalId).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.RunId);
            builder.HasIndex(e => e.ExternalId);
        }
    }
}
