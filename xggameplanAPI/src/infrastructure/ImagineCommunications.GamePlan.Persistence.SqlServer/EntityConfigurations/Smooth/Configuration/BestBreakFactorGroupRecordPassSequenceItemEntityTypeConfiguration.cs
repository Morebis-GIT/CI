using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class BestBreakFactorGroupRecordPassSequenceItemEntityTypeConfiguration : IEntityTypeConfiguration<BestBreakFactorGroupRecordPassSequenceItem>
    {
        public void Configure(EntityTypeBuilder<BestBreakFactorGroupRecordPassSequenceItem> builder)
        {
            builder.ToTable("SmoothBestBreakFactorGroupRecordPassSequences");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();
        }
    }
}
