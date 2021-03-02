using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.PredictionSchedules
{
    public class PredictionScheduleRatingEntityConfiguration : IEntityTypeConfiguration<PredictionScheduleRating>
    {
        public void Configure(EntityTypeBuilder<PredictionScheduleRating> builder)
        {
            builder.ToTable("PredictionScheduleRatings");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.Time).AsUtc();
            builder.Property(e => e.Demographic).HasMaxLength(64).IsRequired();

            builder.HasIndex(x => x.PredictionScheduleId);
            builder.HasIndex(x => x.Demographic);
        }
    }
}
