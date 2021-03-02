using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships
{
    public class SponsorshipAdvertiserExclusivityEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.Sponsorships.SponsorshipAdvertiserExclusivity>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.Sponsorships.SponsorshipAdvertiserExclusivity> builder)
        {
            builder.ToTable("SponsorshipAdvertiserExclusivities");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.SponsoredItemId);
            builder.Property(e => e.AdvertiserIdentifier).HasMaxLength(64);
            builder.Property(e => e.RestrictionType);
            builder.Property(e => e.RestrictionValue);

            builder.HasIndex(x => x.SponsoredItemId);
        }
    }
}
