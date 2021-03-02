using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class TotalRatingEntityConfiguration : IEntityTypeConfiguration<TotalRating>
    {
        public void Configure(EntityTypeBuilder<TotalRating> builder)
        {
            builder.ToTable("TotalRatings");
            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.SalesArea).HasMaxLength(512).IsRequired();
            builder.Property(e => e.Demograph).HasMaxLength(64).IsRequired();
            builder.Property(e => e.Date).AsUtc();
            builder.Property(e => e.TotalRatings).IsRequired();
        }
    }
}
