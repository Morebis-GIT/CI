using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BookingPositionGroups
{
    public class BookingPositionEntityConfiguration : IEntityTypeConfiguration<BookingPosition>
    {
        public void Configure(EntityTypeBuilder<BookingPosition> builder)
        {
            builder.ToTable("BookingPositions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.Abbreviation).HasMaxLength(64);

            builder.HasIndex(e => e.Position).IsUnique();

            builder
                .HasMany<PositionGroupAssociation>()
                .WithOne()
                .HasForeignKey(x => x.BookingPosition)
                .HasPrincipalKey(x => x.Position)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
