using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class LengthFactorEntityConfiguration : IEntityTypeConfiguration<LengthFactor>
    {
        public void Configure(EntityTypeBuilder<LengthFactor> builder)
        {
            _ = builder.ToTable("LengthFactors");

            _ = builder.HasKey(e => e.Id);

            _ = builder.Property(e => e.Id).UseMySqlIdentityColumn();
            _ = builder.Property(e => e.Duration).AsTicks();

            _ = builder.HasOne(x => x.SalesArea).WithMany().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
