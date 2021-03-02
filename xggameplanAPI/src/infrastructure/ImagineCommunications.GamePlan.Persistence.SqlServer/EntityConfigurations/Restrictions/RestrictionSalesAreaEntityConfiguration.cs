using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Restrictions
{
    public class RestrictionSalesAreaEntityConfiguration : IEntityTypeConfiguration<RestrictionSalesArea>
    {
        public void Configure(EntityTypeBuilder<RestrictionSalesArea> builder)
        {
            builder.ToTable("RestrictionsSalesAreas");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.SalesArea).HasMaxLength(64).IsRequired();

            builder.HasIndex(p => p.RestrictionId);
            builder.HasIndex(p => p.SalesArea);
        }
    }
}
