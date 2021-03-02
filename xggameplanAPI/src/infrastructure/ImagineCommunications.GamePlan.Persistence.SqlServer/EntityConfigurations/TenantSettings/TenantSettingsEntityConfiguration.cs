using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.TenantSettings
{
    public class TenantSettingsEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.TenantSettings.TenantSettings>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.TenantSettings.TenantSettings> builder)
        {
            builder.ToTable("TenantSettings");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id);
            builder.Property(e => e.DefaultScenarioId);
            builder.Property(e => e.DefaultSalesAreaPassPriorityId);
            builder.Property(e => e.AutoBookTargetedZeroRatedBreaks);
            builder.Property(e => e.PeakStartTime).AsTicks();
            builder.Property(e => e.PeakEndTime).AsTicks();
            builder.Property(e => e.MidnightStartTime).AsTicks();
            builder.Property(e => e.MidnightEndTime).AsTicks();
            builder.Property(e => e.StartDayOfWeek);
            builder.Property(e => e.Debug);
            builder.Property(e => e.NoOfRatingsPerSalesDayDemo);
            builder.Property(e => e.OpenAirtimeFactor);

            builder.OwnsOne(
                o => o.RunRestrictions,
                ba =>
                {
                    ba.OwnsOne(
                        own => own.MinDocRestriction,
                        bact =>
                        {
                            bact.Property(p => p.Campaigns).HasColumnName("MinimumDocumentRestrictionCampaigns");
                            bact.Property(p => p.Clashes).HasColumnName("MinimumDocumentRestrictionClashes");
                            bact.Property(p => p.ClearanceCodes).HasColumnName("MinimumDocumentRestrictionClearanceCodes");
                            bact.Property(p => p.Demographics).HasColumnName("MinimumDocumentRestrictionDemographics");
                            bact.Property(p => p.Products).HasColumnName("MinimumDocumentRestrictionProducts");
                        }
                    );

                    ba.OwnsOne(
                        own => own.MinRunSizeDocRestriction,
                        bact =>
                        {
                            bact.Property(p => p.Breaks).HasColumnName("MinimumRunSizeDocumentRestrictionBreaks");
                            bact.Property(p => p.Programmes).HasColumnName("MinimumRunSizeDocumentRestrictionProgrammes");
                            bact.Property(p => p.Spots).HasColumnName("MinimumRunSizeDocumentRestrictionSpots");
                        }
                    );
                });

            builder.HasMany(x => x.RunEventSettings)
                .WithOne()
                .HasForeignKey(x => x.TenantSettingsId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Features)
                .WithOne()
                .HasForeignKey(x => x.TenantSettingsId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.WebhookSettings)
                .WithOne()
                .HasForeignKey(x => x.TenantSettingsId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
