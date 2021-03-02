using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Restrictions
{
    public class RestrictionSalesAreaEntityConfiguration : IEntityTypeConfiguration<RestrictionSalesArea>
    {
        public void Configure(EntityTypeBuilder<RestrictionSalesArea> builder)
        {
            _ = builder.ToTable("RestrictionsSalesAreas");

            _ = builder.HasKey(p => p.Id);
            _ = builder.Property(p => p.Id).UseSqlServerIdentityColumn();

            _ = builder.HasIndex(p => p.RestrictionId);
            _ = builder.HasIndex(p => p.SalesAreaId);

            _ = builder.HasOne(x => x.SalesArea)
                .WithMany().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
