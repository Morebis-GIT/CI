using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SalesAreas
{
    public class SalesAreaDemographicEntityConfiguration : IEntityTypeConfiguration<SalesAreaDemographic>
    {
        public void Configure(EntityTypeBuilder<SalesAreaDemographic> builder)
        {
            _ = builder.ToTable("SalesAreaDemographics");
            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(p => p.Id).UseSqlServerIdentityColumn();
            _ = builder.Property(e => e.ExternalRef).HasMaxLength(64).IsRequired();
            _ = builder.Property(e => e.SupplierCode).HasMaxLength(20).IsRequired();

            _ = builder.HasOne(x => x.SalesArea).WithMany().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
