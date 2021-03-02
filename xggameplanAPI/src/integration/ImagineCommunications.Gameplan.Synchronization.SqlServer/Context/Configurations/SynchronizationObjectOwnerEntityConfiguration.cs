using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Entities;

namespace ImagineCommunications.Gameplan.Synchronization.SqlServer.Context.Configurations
{
    public class SynchronizationObjectOwnerEntityConfiguration : IEntityTypeConfiguration<SynchronizationObjectOwner>
    {
        public void Configure(EntityTypeBuilder<SynchronizationObjectOwner> builder)
        {
            builder.ToTable("SynchronizationObjectOwners");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.OwnerId).HasMaxLength(64).IsRequired();
            builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        }
    }
}
