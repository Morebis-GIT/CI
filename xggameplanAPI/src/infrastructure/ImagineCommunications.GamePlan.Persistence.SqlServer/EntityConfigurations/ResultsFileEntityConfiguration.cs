using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ResultsFileEntityConfiguration : IEntityTypeConfiguration<ResultsFile>
    {
        public void Configure(EntityTypeBuilder<ResultsFile> builder)
        {
            builder.ToTable("ResultFiles");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();


            builder.Property(e => e.FileId).HasMaxLength(64).IsRequired();
            builder.Property(e => e.IsCompressed);
            builder.Property(e => e.FileContent).IsRequired();

            builder.HasIndex(new[] {"ScenarioId", "FileId"}).IsUnique();
        }
    }
}
