using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassRuleBaseEntityConfiguration : IEntityTypeConfiguration<PassRuleBase>
    {
        public void Configure(EntityTypeBuilder<PassRuleBase> builder)
        {
            builder.ToTable("PassRules")
                .HasDiscriminator<int>("Discriminator")
                .HasValue<PassRuleGeneral>((int)PassRuleType.General)
                .HasValue<PassRule>((int)PassRuleType.Rule)
                .HasValue<PassRuleTolerance>((int)PassRuleType.Tolerance)
                .HasValue<PassRuleWeighting>((int)PassRuleType.Weighting);
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.Description).HasMaxLength(512);
            builder.Property(e => e.InternalType).HasMaxLength(64);
            builder.Property(e => e.Value).HasMaxLength(256);
            builder.Property(e => e.Type).HasMaxLength(64);
            builder.Property(e => e.PassId);

            builder.HasIndex(x => x.Type);
        }
    }

    public class PassToleranceEntityConfiguration : IEntityTypeConfiguration<PassRuleTolerance>
    {
        public void Configure(EntityTypeBuilder<PassRuleTolerance> builder)
        {
            builder.Property(e => e.Ignore).HasColumnName("Ignore");
            builder.Property(e => e.Under).HasColumnName("Under");
            builder.Property(e => e.Over).HasColumnName("Over");
            builder.Property(e => e.ForceOverUnder).HasColumnName("ForceOverUnder");
            builder.Property(e => e.BookTargetArea).HasColumnName("BookTargetArea");
            builder.Property(e => e.CampaignType).HasColumnName("CampaignType");
        }
    }

    public class PassRuleEntityConfiguration : IEntityTypeConfiguration<PassRule>
    {
        public void Configure(EntityTypeBuilder<PassRule> builder)
        {
            builder.Property(e => e.Ignore).HasColumnName("Ignore");
            builder.Property(e => e.PeakValue).HasMaxLength(64).HasColumnName("PeakValue");
            builder.Property(e => e.CampaignType).HasColumnName("CampaignType");
        }
    }
}
