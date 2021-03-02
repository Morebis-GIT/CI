using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class PreviewFileEntityConfiguration : IEntityTypeConfiguration<PreviewFile>
    {
        public virtual void Configure(EntityTypeBuilder<PreviewFile> builder)
        {
            builder.ToTable("Previews");

            builder.HasKey(k => k.Id);

            builder.Property(e => e.Location).HasMaxLength(256);
            builder.Property(e => e.FileName).IsRequired().HasMaxLength(128);
            builder.Property(e => e.ContentLength).IsRequired();
            builder.Property(e => e.ContentType).IsRequired().HasMaxLength(64);
            builder.Property(e => e.FileExtension).HasMaxLength(64);
            builder.Property(e => e.UserId);
            builder.Property(e => e.TenantId);
            builder.Property(e => e.Content);

            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.TenantId);
        }
    }
}
