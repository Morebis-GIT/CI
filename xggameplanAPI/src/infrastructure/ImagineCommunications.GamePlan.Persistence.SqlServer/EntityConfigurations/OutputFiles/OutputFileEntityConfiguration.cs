using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.OutputFiles
{
    public class OutputFileEntityConfiguration : IEntityTypeConfiguration<OutputFile>
    {
        public void Configure(EntityTypeBuilder<OutputFile> builder)
        {
            builder.ToTable("OutputFiles");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.FileId).HasMaxLength(64).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(256);
            builder.Property(e => e.AutoBookFileName).HasMaxLength(64).IsRequired();

            builder.HasMany(e => e.Columns).WithOne()
                .HasForeignKey(e => e.OutputFileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.FileId).IsUnique();
        }
    }
}
