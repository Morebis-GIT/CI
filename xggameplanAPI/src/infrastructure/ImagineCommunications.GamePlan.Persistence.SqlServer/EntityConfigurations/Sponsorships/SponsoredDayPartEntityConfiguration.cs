using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships
{
    public class SponsoredDayPartEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.Sponsorships.SponsoredDayPart>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.Sponsorships.SponsoredDayPart> builder)
        {
            builder.ToTable("SponsoredDayParts");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.SponsorshipItemId);
            builder.Property(e => e.StartTime).AsTicks();
            builder.Property(e => e.EndTime).AsTicks();
            builder.Property(e => e.DaysOfWeek).AsStringPattern(DayOfWeek.Monday);

            builder.HasIndex(x => x.SponsorshipItemId);
        }
    }
}
