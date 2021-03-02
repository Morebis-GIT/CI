using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters
{
    public class AgMultiPartEntityConfiguration : IEntityTypeConfiguration<AgMultiPart>
    {
        public void Configure(EntityTypeBuilder<AgMultiPart> builder)
        {
            builder.ToTable("AgMultiParts");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(e => e.AgLengthId);
        }
    }
}
