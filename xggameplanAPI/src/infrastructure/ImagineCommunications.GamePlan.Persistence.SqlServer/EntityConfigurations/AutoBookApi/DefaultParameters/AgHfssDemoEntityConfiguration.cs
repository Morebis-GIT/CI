using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgHfssDemoEntityConfiguration : IEntityTypeConfiguration<AgHfssDemo>
    {
        public void Configure(EntityTypeBuilder<AgHfssDemo> builder)
        {
            builder.ToTable("AgHfssDemos");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.BreakScheduledDate).HasMaxLength(32);
        }
    }
}
