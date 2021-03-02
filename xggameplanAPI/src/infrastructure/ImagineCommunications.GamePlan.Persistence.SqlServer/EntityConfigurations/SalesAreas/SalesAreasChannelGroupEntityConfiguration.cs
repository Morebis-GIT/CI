using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SalesAreas
{
    public class SalesAreasChannelGroupEntityConfiguration : IEntityTypeConfiguration<SalesAreasChannelGroup>
    {
        public void Configure(EntityTypeBuilder<SalesAreasChannelGroup> builder)
        {
            builder.ToTable("SalesAreasChannelGroups");
            builder.HasKey(e => e.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();
            builder.Property(x => x.Name).HasMaxLength(256);
            builder.HasIndex(e => e.SalesAreaId);
        }
    }
}
