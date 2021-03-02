using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships
{
    public class SponsoredItemEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.Sponsorships.SponsoredItem>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.Sponsorships.SponsoredItem> builder)
        {
            builder.ToTable("SponsoredItems");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.SponsorshipId);
            builder.Property(e => e.CalculationType);
            builder.Property(e => e.RestrictionType);
            builder.Property(e => e.RestrictionValue);
            builder.Property(e => e.Applicability);
            builder.Property(e => e.Products).AsDelimitedString();

            builder.HasMany(e => e.SponsorshipItems).WithOne()
                .HasForeignKey(e => e.SponsoredItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.ClashExclusivities).WithOne()
                .HasForeignKey(e => e.SponsoredItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AdvertiserExclusivities).WithOne()
                .HasForeignKey(e => e.SponsoredItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.SponsorshipId);
        }
    }
}
