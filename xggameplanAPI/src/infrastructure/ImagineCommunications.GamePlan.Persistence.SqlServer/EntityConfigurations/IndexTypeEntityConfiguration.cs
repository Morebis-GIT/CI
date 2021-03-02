using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class IndexTypeEntityConfiguration : IEntityTypeConfiguration<IndexType>
    {
        public void Configure(EntityTypeBuilder<IndexType> builder)
        {
            _ = builder.ToTable("IndexTypes");
            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            _ = builder.Property(e => e.BaseDemographicNo).HasMaxLength(64);
            _ = builder.Property(e => e.DemographicNo).HasMaxLength(64);
            _ = builder.Property(e => e.Description).HasMaxLength(512);

            _ = builder.HasIndex(e => e.BaseDemographicNo);
            _ = builder.HasIndex(e => e.DemographicNo);
            _ = builder.HasIndex(e => e.SalesAreaId);

            _ = builder.HasOne<SalesArea>().WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
