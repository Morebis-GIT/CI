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
            _ = builder.ToTable("TotalRatings");
            _ = builder.HasKey(k => k.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            _ = builder.Property(e => e.Demograph).HasMaxLength(64).IsRequired();
            _ = builder.Property(e => e.Date).AsUtc();
            _ = builder.Property(e => e.TotalRatings).IsRequired();
            _ = builder.HasOne(x => x.SalesArea).WithMany().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
