using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BookingPositionGroups
{
    public class PositionGroupAssociationEntityConfiguration : IEntityTypeConfiguration<PositionGroupAssociation>
    {
        public void Configure(EntityTypeBuilder<PositionGroupAssociation> builder)
        {
            builder.ToTable("PositionGroupAssociations");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.HasIndex(x => new {x.BookingPosition, x.BookingPositionGroupId}).IsUnique();
        }
    }
}
