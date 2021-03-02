using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class UpdateDetailsEntityConfiguration : IEntityTypeConfiguration<UpdateDetails>
    {
        public virtual void Configure(EntityTypeBuilder<UpdateDetails> builder)
        {
            builder.ToTable("UpdateDetails");
            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("newid()");
            builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
            builder.Property(e => e.TenantId).IsRequired().HasDefaultValue(0);
            builder.Property(e => e.TimeApplied).IsRequired().AsUtc();
        }
    }
}
