using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgCampaignSalesAreaEntityConfiguration : IEntityTypeConfiguration<AgCampaignSalesArea>
    {
        public void Configure(EntityTypeBuilder<AgCampaignSalesArea> builder)
        {
            builder.ToTable("AgCampaignSalesAreas");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasMany(e => e.AgLengths).WithOne()
                .HasForeignKey(e => e.AgCampaignSalesAreaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgStrikeWeights).WithOne()
                .HasForeignKey(e => e.AgCampaignSalesAreaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgDayParts).WithOne()
                .HasForeignKey(e => e.AgCampaignSalesAreaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgParts).WithOne()
                .HasForeignKey(e => e.AgCampaignSalesAreaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AgPartsLengths).WithOne()
                .HasForeignKey(e => e.AgCampaignSalesAreaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.Type)
                .HasFilter($"{nameof(AgCampaignSalesArea.Type)} = {(int)NestedType.TypeMember}")
                .IsUnique(true);

            builder.HasIndex(e => e.AutoBookDefaultParametersId);
        }
    }
}
