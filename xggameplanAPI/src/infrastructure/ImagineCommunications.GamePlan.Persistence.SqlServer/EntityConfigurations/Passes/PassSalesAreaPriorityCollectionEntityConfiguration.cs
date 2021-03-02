using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassSalesAreaPriorityCollectionEntityConfiguration : IEntityTypeConfiguration<PassSalesAreaPriorityCollection>
    {
        public void Configure(EntityTypeBuilder<PassSalesAreaPriorityCollection> builder)
        {
            builder.ToTable("PassSalesAreaPriorityCollection");

            builder.HasKey(e => e.Id);

            builder.Property(p => p.DaysOfWeek).HasMaxLength(7);
            builder.Property(p => p.StartDate).AsUtc();
            builder.Property(p => p.EndDate).AsUtc();
            builder.Property(p => p.StartTime).AsTicks();
            builder.Property(p => p.EndTime).AsTicks();
            builder.Property(p => p.IsPeakTime).HasDefaultValue(false);
            builder.Property(p => p.IsOffPeakTime).HasDefaultValue(false);
            builder.Property(p => p.IsMidnightTime).HasDefaultValue(false);

            builder.HasMany(x => x.SalesAreaPriorities).WithOne()
                .HasForeignKey(x => x.PassSalesAreaPriorityCollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.PassId);
        }
    }
}
