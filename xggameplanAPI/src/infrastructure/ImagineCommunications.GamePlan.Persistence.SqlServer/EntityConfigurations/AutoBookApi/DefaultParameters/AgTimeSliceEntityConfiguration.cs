using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgTimeSliceEntityConfiguration : IEntityTypeConfiguration<AgTimeSlice>
    {
        public void Configure(EntityTypeBuilder<AgTimeSlice> builder)
        {
            builder.ToTable("AgTimeSlices");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.StartDate).HasMaxLength(32);
            builder.Property(e => e.StartTime).HasMaxLength(32);
            builder.Property(e => e.EndTime).HasMaxLength(32);

            builder.HasIndex(e => e.AgDayPartId);
        }
    }
}
