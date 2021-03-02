using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth
{
    public class SmoothFailuresSmoothFailureMessagesEntityTypeConfiguration : IEntityTypeConfiguration<SmoothFailureSmoothFailureMessage>
    {
        public void Configure(EntityTypeBuilder<SmoothFailureSmoothFailureMessage> builder)
        {
            builder.ToTable("SmoothFailuresSmoothFailureMessages");
            
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();

            builder.HasIndex(x => new {x.SmoothFailureId, x.SmoothFailureMessageId}).IsUnique();
        }
    }
}
