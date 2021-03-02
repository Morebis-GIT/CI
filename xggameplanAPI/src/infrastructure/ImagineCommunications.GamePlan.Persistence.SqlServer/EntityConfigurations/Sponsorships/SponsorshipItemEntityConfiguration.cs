using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships
{
    public class SponsorshipItemEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.Sponsorships.SponsorshipItem>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.Sponsorships.SponsorshipItem> builder)
        {
            builder.ToTable("SponsorshipItems");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.SponsoredItemId);
            builder.Property(e => e.StartDate).AsUtc();
            builder.Property(e => e.EndDate).AsUtc();
            builder.Property(e => e.ProgrammeName).HasMaxLength(128);
            builder.Property(e => e.SalesAreas).AsDelimitedString();

            builder.HasMany(e => e.DayParts).WithOne()
                .HasForeignKey(e => e.SponsorshipItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.SponsoredItemId);
        }
    }
}
