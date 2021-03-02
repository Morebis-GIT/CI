using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.FunctionalAreas
{
    public class FaultTypeEntityConfiguration : IEntityTypeConfiguration<FaultType>
    {
        public void Configure(EntityTypeBuilder<FaultType> builder)
        {
            builder.ToTable("FaultTypes");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.FunctionalAreaFaultType).WithOne(x => x.FaultType)
                .HasPrincipalKey<FaultType>(x => x.Id);
            builder.HasMany(x => x.Descriptions).WithOne().HasForeignKey(x => x.FaultTypeId);
        }
    }
}
