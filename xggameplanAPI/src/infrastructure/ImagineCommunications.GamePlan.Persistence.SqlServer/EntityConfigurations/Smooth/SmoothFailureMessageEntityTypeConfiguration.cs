using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth
{
    public class SmoothFailureMessageEntityTypeConfiguration : IEntityTypeConfiguration<SmoothFailureMessage>
    {
        public void Configure(EntityTypeBuilder<SmoothFailureMessage> builder)
        {
            builder.ToTable("SmoothFailureMessages");

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Descriptions).WithOne().HasForeignKey(x => x.SmoothFailureMessageId);
        }
    }
}
