using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SmoothPassIterationRecordPassSequenceItemEntityTypeConfiguration: IEntityTypeConfiguration<SmoothPassIterationRecordPassSequenceItem>
    {
        public void Configure(EntityTypeBuilder<SmoothPassIterationRecordPassSequenceItem> builder)
        {
            builder.ToTable("SmoothPassIterationRecordPassSequences");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseSqlServerIdentityColumn();
        }
    }
}
