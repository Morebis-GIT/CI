using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships
{
    public class SponsorshipClashExclusivityEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.Sponsorships.SponsorshipClashExclusivity>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.Sponsorships.SponsorshipClashExclusivity> builder)
        {
            builder.ToTable("SponsorshipClashExclusivities");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.SponsoredItemId);
            builder.Property(e => e.ClashExternalRef).HasMaxLength(64);
            builder.Property(e => e.RestrictionType);
            builder.Property(e => e.RestrictionValue);

            builder.HasIndex(x => x.SponsoredItemId);

            builder.HasIndex(x => x.ClashExternalRef);
        }
    }
}
