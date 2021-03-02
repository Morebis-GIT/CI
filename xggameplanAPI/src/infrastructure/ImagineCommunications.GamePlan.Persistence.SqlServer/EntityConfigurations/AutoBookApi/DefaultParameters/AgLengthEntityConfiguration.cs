using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgLengthEntityConfiguration : IEntityTypeConfiguration<AgLength>
    {
        public void Configure(EntityTypeBuilder<AgLength> builder)
        {
            builder.ToTable("AgLengths");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.HasMany(e => e.AgMultiParts).WithOne()
                .HasForeignKey(e => e.AgLengthId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.AgCampaignSalesAreaId);
        }
    }
}
