using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi
{
    public class AutoBookInstanceConfigurationEntityConfiguration : IEntityTypeConfiguration<AutoBookInstanceConfiguration>
    {
        public void Configure(EntityTypeBuilder<AutoBookInstanceConfiguration> builder)
        {
            builder.ToTable("AutoBookInstanceConfigurations");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Description).HasMaxLength(256);

            builder.HasMany(x => x.CriteriaList).WithOne()
                .HasForeignKey(x => x.AutoBookInstanceConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
