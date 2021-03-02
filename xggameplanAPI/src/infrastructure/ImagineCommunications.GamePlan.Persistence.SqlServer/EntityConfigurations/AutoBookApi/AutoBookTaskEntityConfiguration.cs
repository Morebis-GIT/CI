using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi
{
    public class AutoBookTaskEntityConfiguration : IEntityTypeConfiguration<AutoBookTask>
    {
        public void Configure(EntityTypeBuilder<AutoBookTask> builder)
        {
            builder.ToTable("AutoBookTasks");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(e => e.AutoBookId);
        }
    }
}
