using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassSalesAreaPriorityEntityConfiguration : IEntityTypeConfiguration<PassSalesAreaPriority>
    {
        public void Configure(EntityTypeBuilder<PassSalesAreaPriority> builder)
        {
            _ = builder.ToTable("PassSalesAreaPriorities");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.HasOne(x => x.SalesArea)
                .WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(x => x.SalesAreaId);
            _ = builder.HasIndex(x => x.PassSalesAreaPriorityCollectionId);
        }
    }
}
