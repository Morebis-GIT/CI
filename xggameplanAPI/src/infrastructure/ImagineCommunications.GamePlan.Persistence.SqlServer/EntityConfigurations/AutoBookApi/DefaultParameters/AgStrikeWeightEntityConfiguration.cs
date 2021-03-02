using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgStrikeWeightEntityConfiguration : IEntityTypeConfiguration<AgStrikeWeight>
    {
        public void Configure(EntityTypeBuilder<AgStrikeWeight> builder)
        {
            builder.ToTable("AgStrikeWeights");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.StartDate).HasMaxLength(32);
            builder.Property(e => e.EndDate).HasMaxLength(32);

            builder.HasMany(e => e.AgStrikeWeightLengths).WithOne()
                .HasForeignKey(e => e.AgStrikeWeightId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.AgCampaignSalesAreaId);
        }
    }
}
