using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships
{
    public class SponsorshipItemEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.Sponsorships.SponsorshipItem>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.Sponsorships.SponsorshipItem> builder)
        {
            _ = builder.ToTable("SponsorshipItems");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.Property(e => e.SponsoredItemId);
            _ = builder.Property(e => e.StartDate).AsUtc();
            _ = builder.Property(e => e.EndDate).AsUtc();
            _ = builder.Property(e => e.ProgrammeName).HasMaxLength(128);

            _ = builder.HasMany(e => e.DayParts).WithOne()
                .HasForeignKey(e => e.SponsorshipItemId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasMany(e => e.SalesAreas).WithOne(e => e.SponsorshipItem);

            _ = builder.HasIndex(x => x.SponsoredItemId);
        }
    }
}
