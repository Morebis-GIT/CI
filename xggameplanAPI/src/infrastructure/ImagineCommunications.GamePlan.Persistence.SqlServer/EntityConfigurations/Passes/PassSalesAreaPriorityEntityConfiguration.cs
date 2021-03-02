using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassSalesAreaPriorityEntityConfiguration : IEntityTypeConfiguration<PassSalesAreaPriority>
    {
        public void Configure(EntityTypeBuilder<PassSalesAreaPriority> builder)
        {
            builder.ToTable("PassSalesAreaPriorities");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.SalesArea).IsRequired().HasMaxLength(64);

            builder.HasIndex(x => x.PassSalesAreaPriorityCollectionId);
        }
    }
}
