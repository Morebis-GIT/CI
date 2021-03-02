using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class TaskInstanceEntityConfiguration : IEntityTypeConfiguration<TaskInstance>
    {
        public virtual void Configure(EntityTypeBuilder<TaskInstance> builder)
        {
            builder.ToTable("TaskInstances");

            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id);

            builder.Property(e => e.TaskId).IsRequired().HasMaxLength(128);
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.TimeCompleted).AsUtc();
            builder.Property(e => e.TimeLastActive).AsUtc();
            builder.Property(e => e.TimeCreated).IsRequired().AsUtc();
            builder.Property(e => e.TenantId).HasDefaultValue(0);


            builder.HasMany(e => e.Parameters)
                .WithOne()
                .HasForeignKey(e => e.TaskInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.TaskId);
        }
    }
}
