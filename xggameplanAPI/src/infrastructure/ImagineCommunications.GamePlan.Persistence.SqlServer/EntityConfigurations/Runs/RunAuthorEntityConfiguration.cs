using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class RunAuthorEntityConfiguration : IEntityTypeConfiguration<RunAuthor>
    {
        public void Configure(EntityTypeBuilder<RunAuthor> builder)
        {
            builder.ToTable("RunAuthors");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();


            builder.Property(p => p.Name).HasMaxLength(64).IsRequired();

            builder.HasIndex(e => e.RunId);
        }
    }
}
