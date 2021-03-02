using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class SalesAreaPriorityEntityConfiguration : IEntityTypeConfiguration<SalesAreaPriority>
    {
        public void Configure(EntityTypeBuilder<SalesAreaPriority> builder)
        {
            _ = builder.ToTable("SalesAreaPriorities");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseMySqlIdentityColumn();

            _ = builder.Property(e => e.Priority).IsRequired();

            _ = builder.HasIndex(e => e.LibrarySalesAreaPassPriorityUid);
            _ = builder.HasIndex(e => e.SalesAreaId);

            _ = builder.HasOne(x => x.SalesArea).WithMany().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
