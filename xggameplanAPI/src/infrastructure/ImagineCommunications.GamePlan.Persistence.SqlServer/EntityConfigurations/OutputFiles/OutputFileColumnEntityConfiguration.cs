using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.OutputFiles
{
    public class OutputFileColumnEntityConfiguration : IEntityTypeConfiguration<OutputFileColumn>
    {
        public void Configure(EntityTypeBuilder<OutputFileColumn> builder)
        {
            builder.ToTable("OutputFileColumns");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(256);
            builder.Property(e => e.DataType).HasMaxLength(64);

            builder.HasIndex(e => e.OutputFileId);
        }
    }
}
