using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ClashDifferenceEntityTypeConfiguration : IEntityTypeConfiguration<ClashDifference>
    {
        public void Configure(EntityTypeBuilder<ClashDifference> builder)
        {
            _ = builder.ToTable("ClashDifferences");
            _ = builder.HasKey(x => x.Id);
            _ = builder.Property(x => x.Id).UseMySqlIdentityColumn();
            _ = builder.Property(x => x.StartDate).AsUtc();
            _ = builder.Property(x => x.EndDate).AsUtc();
            _ = builder.HasIndex(x => x.ClashId);
            _ = builder.HasIndex(x => x.SalesAreaId);

            _ = builder.HasOne(e => e.TimeAndDow).WithOne()
                .HasForeignKey<ClashDifferenceTimeAndDow>(e => e.ClashDifferenceId);

            _ = builder.HasOne(x => x.SalesArea).WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
