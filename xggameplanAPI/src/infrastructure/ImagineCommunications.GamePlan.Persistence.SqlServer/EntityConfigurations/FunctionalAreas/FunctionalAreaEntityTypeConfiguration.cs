using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.FunctionalAreas
{
    public class FunctionalAreaEntityTypeConfiguration : IEntityTypeConfiguration<FunctionalArea>
    {
        public void Configure(EntityTypeBuilder<FunctionalArea> builder)
        {
            builder.ToTable("FunctionalAreas");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.HasMany(x => x.Descriptions).WithOne().HasForeignKey(x => x.FunctionalAreaId);
            builder.HasMany(x => x.FunctionalAreaFaultTypes).WithOne().HasForeignKey(x => x.FunctionalAreaId);
        }
    }
}
