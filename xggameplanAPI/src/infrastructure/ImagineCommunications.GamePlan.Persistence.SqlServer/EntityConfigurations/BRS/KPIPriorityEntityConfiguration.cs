using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BRS
{
    public class KPIPriorityEntityConfiguration : IEntityTypeConfiguration<KPIPriority>
    {
        public void Configure(EntityTypeBuilder<KPIPriority> builder)
        {
            builder.ToTable("KPIPriorities");
            builder.HasKey(k => k.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        }
    }
}
