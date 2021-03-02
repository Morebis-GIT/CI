using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgDayPartLengthEntityConfiguration : IEntityTypeConfiguration<AgDayPartLength>
    {
        public void Configure(EntityTypeBuilder<AgDayPartLength> builder)
        {
            builder.ToTable("AgDayPartLengths");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.StartDate).HasMaxLength(32);

            builder.HasIndex(e => e.AgDayPartId);
        }
    }
}
