using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SmoothPassUnplacedIterationEntityTypeConfiguration : IEntityTypeConfiguration<SmoothPassUnplacedIteration>
    {
        public void Configure(EntityTypeBuilder<SmoothPassUnplacedIteration> builder)
        {
            builder.ToTable("SmoothPassUnplacedIterations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseMySqlIdentityColumn();
        }
    }
}
