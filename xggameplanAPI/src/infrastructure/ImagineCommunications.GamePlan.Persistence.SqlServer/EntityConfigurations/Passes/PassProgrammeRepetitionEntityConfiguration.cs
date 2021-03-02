using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassProgrammeRepetitionEntityConfiguration : IEntityTypeConfiguration<PassProgrammeRepetition>
    {
        public void Configure(EntityTypeBuilder<PassProgrammeRepetition> builder)
        {
            builder.ToTable("PassProgrammeRepetitions");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.HasIndex(x => x.PassId);
        }
    }
}
