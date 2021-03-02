using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgDayPartEntityConfiguration : IEntityTypeConfiguration<AgDayPart>
    {
        public void Configure(EntityTypeBuilder<AgDayPart> builder)
        {
            builder.ToTable("AgDayParts");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.StartDate).HasMaxLength(32);
            builder.Property(e => e.EndDate).HasMaxLength(32);

            builder.HasMany(e => e.AgTimeSlices).WithOne()
                .HasForeignKey(e => e.AgDayPartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgDayPartLengths).WithOne()
                .HasForeignKey(e => e.AgDayPartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.AgCampaignSalesAreaId);
        }
    }
}
