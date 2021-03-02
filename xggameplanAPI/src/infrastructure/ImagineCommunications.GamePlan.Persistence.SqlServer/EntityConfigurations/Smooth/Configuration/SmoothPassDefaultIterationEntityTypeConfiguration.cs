using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SmoothPassDefaultIterationEntityTypeConfiguration : IEntityTypeConfiguration<SmoothPassDefaultIteration>
    {
        public void Configure(EntityTypeBuilder<SmoothPassDefaultIteration> builder)
        {
            builder.ToTable("SmoothPassDefaultIterations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseMySqlIdentityColumn();
        }
    }
}
