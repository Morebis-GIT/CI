using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.FunctionalAreas
{
    public class FunctionalAreaFaultTypeEntityConfiguration : IEntityTypeConfiguration<FunctionalAreaFaultType>
    {
        public void Configure(EntityTypeBuilder<FunctionalAreaFaultType> builder)
        {
            builder.ToTable("FunctionalAreaFaultTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.FaultTypeId).IsRequired();
            builder.Property(x => x.FunctionalAreaId).IsRequired();
            builder.Property(x => x.IsSelected).IsRequired().HasDefaultValue(true);

            builder.HasIndex(x => x.FaultTypeId);
            builder.HasIndex(x => x.FunctionalAreaId);

            builder.HasOne(x => x.FaultType).WithOne(x => x.FunctionalAreaFaultType)
                .HasForeignKey<FunctionalAreaFaultType>(x => x.FaultTypeId);
        }
    }
}
