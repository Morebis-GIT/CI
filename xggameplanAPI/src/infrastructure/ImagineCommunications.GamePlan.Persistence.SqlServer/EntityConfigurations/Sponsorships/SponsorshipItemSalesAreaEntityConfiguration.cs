using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships
{
    public class SponsorshipItemSalesAreaEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.Sponsorships.SponsorshipItemSalesArea>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.Sponsorships.SponsorshipItemSalesArea> builder)
        {
            _ = builder.ToTable("SponsorshipItemsSalesAreas");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            
            _ = builder.HasIndex(x => x.SponsorshipItemId);
            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
