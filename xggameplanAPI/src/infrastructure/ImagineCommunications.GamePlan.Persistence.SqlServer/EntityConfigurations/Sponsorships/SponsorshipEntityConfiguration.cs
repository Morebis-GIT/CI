using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships
{
    public class SponsorshipEntityConfiguration : AuditEntityTypeConfiguration<Entities.Tenant.Sponsorships.Sponsorship>
    {
        public override void Configure(EntityTypeBuilder<Entities.Tenant.Sponsorships.Sponsorship> builder)
        {
            builder.ToTable("Sponsorships");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.Uid).IsRequired();
            builder.Property(e => e.ExternalReferenceId).HasMaxLength(64);
            builder.Property(e => e.RestrictionLevel);

            builder.HasMany(e => e.SponsoredItems).WithOne()
                .HasForeignKey(e => e.SponsorshipId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ExternalReferenceId);
        }
    }
}
