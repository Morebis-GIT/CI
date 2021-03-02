using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BookingPositionGroups
{
    public class BookingPositionGroupEntityConfiguration : IEntityTypeConfiguration<BookingPositionGroup>
    {
        public void Configure(EntityTypeBuilder<BookingPositionGroup> builder)
        {
            builder.ToTable("BookingPositionGroups");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.Code).HasMaxLength(64);
            builder.Property(x => x.Description).HasMaxLength(256);

            builder.HasIndex(e => e.GroupId).IsUnique();

            builder
                .HasMany(x => x.PositionGroupAssociations)
                .WithOne()
                .HasForeignKey(x => x.BookingPositionGroupId)
                .HasPrincipalKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
