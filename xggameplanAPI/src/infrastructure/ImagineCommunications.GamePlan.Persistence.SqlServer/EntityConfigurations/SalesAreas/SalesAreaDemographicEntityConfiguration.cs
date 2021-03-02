using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SalesAreas
{
    public class SalesAreaDemographicEntityConfiguration : IEntityTypeConfiguration<SalesAreaDemographic>
    {
        public void Configure(EntityTypeBuilder<SalesAreaDemographic> builder)
        {
            builder.ToTable("SalesAreaDemographics");
            builder.HasKey(e => e.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.SalesArea).HasMaxLength(255).IsRequired();
            builder.Property(e => e.ExternalRef).HasMaxLength(64).IsRequired();
            builder.Property(e => e.SupplierCode).HasMaxLength(20).IsRequired();

            builder
                .HasOne<SalesArea>()
                .WithMany()
                .HasForeignKey(x => x.SalesArea)
                .HasPrincipalKey(x => x.Name);
        }
    }
}
