using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassRatingPointSalesAreaRefEntityConfiguration : IEntityTypeConfiguration<PassRatingPointSalesAreaRef>
    {
        public void Configure(EntityTypeBuilder<PassRatingPointSalesAreaRef> builder)
        {
            _ = builder.ToTable("PassRatingPoint_SalesAreaRefs");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.HasOne(x => x.SalesArea)
                .WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(x => x.SalesAreaId);
            _ = builder.HasIndex(x => x.PassRatingPointId);
        }
    }
}
