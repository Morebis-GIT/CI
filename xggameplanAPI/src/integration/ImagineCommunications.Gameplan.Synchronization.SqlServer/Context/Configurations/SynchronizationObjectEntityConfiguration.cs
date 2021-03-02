using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Entities;

namespace ImagineCommunications.Gameplan.Synchronization.SqlServer.Context.Configurations
{
    public class SynchronizationObjectEntityConfiguration : IEntityTypeConfiguration<SynchronizationObject>
    {
        public void Configure(EntityTypeBuilder<SynchronizationObject> builder)
        {
            builder.ToTable("SynchronizationObjects");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.RowVersion).IsRowVersion();
            builder.Property(e => e.OwnerCount).IsRequired().HasDefaultValue(0);

            builder.HasMany(e => e.Owners)
                .WithOne()
                .HasForeignKey(e => e.SynchronizationObjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
