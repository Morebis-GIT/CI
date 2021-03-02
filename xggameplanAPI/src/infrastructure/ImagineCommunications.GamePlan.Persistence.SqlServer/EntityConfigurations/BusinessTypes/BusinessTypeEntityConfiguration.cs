using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BusinessTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BusinessTypes
{
    public class BusinessTypeEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.BusinessTypes.BusinessType>
    {
        public void Configure(EntityTypeBuilder<BusinessType> builder)
        {
            builder.ToTable("BusinessTypes");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.HasIndex(e => e.Code)
                .IsUnique();
        }
    }
}
