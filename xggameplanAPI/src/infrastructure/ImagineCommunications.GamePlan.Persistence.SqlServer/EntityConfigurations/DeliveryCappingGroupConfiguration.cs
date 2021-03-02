using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class DeliveryCappingGroupConfiguration : IEntityTypeConfiguration<DeliveryCappingGroup>
    {
        public void Configure(EntityTypeBuilder<DeliveryCappingGroup> builder)
        {
            builder.ToTable("DeliveryCappingGroup");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            builder.Property(e => e.Description).HasMaxLength(256).IsRequired();
        }
    }
}
