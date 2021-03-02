using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class TaskInstanceParameterEntityConfiguration : IEntityTypeConfiguration<TaskInstanceParameter>
    {
        public virtual void Configure(EntityTypeBuilder<TaskInstanceParameter> builder)
        {
            builder.ToTable("TaskInstanceParameters");

            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.TaskInstanceId).IsRequired().HasMaxLength(128);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
            builder.Property(e => e.Value).HasMaxLength(2048);

            builder.HasIndex(e => e.TaskInstanceId);
        }
    }
}
