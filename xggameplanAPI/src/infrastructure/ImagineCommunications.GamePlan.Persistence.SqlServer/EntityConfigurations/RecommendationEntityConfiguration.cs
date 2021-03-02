using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class RecommendationEntityConfiguration : IEntityTypeConfiguration<Recommendation>
    {
        public void Configure(EntityTypeBuilder<Recommendation> builder)
        {
            builder.ToTable("Recommendations");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.ScenarioId).HasMaxLength(64);
            builder.Property(p => p.ExternalSpotRef).HasMaxLength(64);
            builder.Property(p => p.ExternalCampaignNumber).HasMaxLength(64);
            builder.Property(e => e.SpotLength).AsTicks();
            builder.Property(e => e.Product).HasMaxLength(64);
            builder.Property(e => e.Demographic).HasMaxLength(64);
            builder.Property(p => p.BreakBookingPosition);
            builder.Property(p => p.StartDateTime).AsUtc();
            builder.Property(e => e.SalesArea).HasMaxLength(64);
            builder.Property(e => e.ExternalProgrammeReference).HasMaxLength(64);
            builder.Property(e => e.BreakType).HasMaxLength(32);
            builder.Property(p => p.SpotRating).HasColumnType("DECIMAL(28,18)");
            builder.Property(p => p.SpotEfficiency);
            builder.Property(e => e.Action).HasMaxLength(1);
            builder.Property(e => e.Processor).HasMaxLength(64);
            builder.Property(p => p.ProcessorDateTime).AsUtc();
            builder.Property(e => e.GroupCode).HasMaxLength(64);
            builder.Property(p => p.EndDateTime).AsUtc();
            builder.Property(p => p.ClientPicked);
            builder.Property(e => e.MultipartSpot).HasMaxLength(64);
            builder.Property(e => e.MultipartSpotPosition).HasMaxLength(64);
            builder.Property(e => e.MultipartSpotRef);
            builder.Property(e => e.RequestedPositionInBreak).HasMaxLength(64);
            builder.Property(e => e.ActualPositionInBreak).HasMaxLength(64);
            builder.Property(e => e.ExternalBreakNo).HasMaxLength(64);
            builder.Property(p => p.Filler);
            builder.Property(p => p.Sponsored);
            builder.Property(p => p.Preemptable);
            builder.Property(p => p.Preemptlevel);
            builder.Property(p => p.PassSequence);
            builder.Property(p => p.PassIterationSequence);
            builder.Property(e => e.PassName).HasMaxLength(256);
            builder.Property(p => p.OptimiserPassSequenceNumber);
            builder.Property(p => p.CampaignPassPriority);
            builder.Property(p => p.RankOfEfficiency);
            builder.Property(p => p.RankOfCampaign);
            builder.Property(p => p.CampaignWeighting);
            builder.Property(p => p.SpotSequenceNumber);

            builder.HasIndex(p => p.Processor);
            builder.HasIndex(p => p.ScenarioId);
        }
    }
}
