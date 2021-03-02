using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class AccessTokenEntityConfiguration : IEntityTypeConfiguration<AccessToken>
    {
        public virtual void Configure(EntityTypeBuilder<AccessToken> builder)
        {
            builder.ToTable("AccessTokens");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.Token).HasMaxLength(128).IsRequired();
            builder.Property(p => p.ValidUntilValue).IsRequired().AsUtc();
            builder.Property(p => p.UserId).HasDefaultValue(0);

            builder.HasIndex(e => e.Token);
        }
    }
}
