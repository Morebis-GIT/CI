using System;
using System.Linq.Expressions;
using System.Text;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Programmes
{
    public class ProgrammeEntityConfiguration : IEntityTypeConfiguration<Programme>
    {
        public void Configure(EntityTypeBuilder<Programme> builder)
        {
            _ = builder.ToTable("Programmes");
            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).HasDefaultValueSql("newid()");
            _ = builder.Property(e => e.StartDateTime).AsUtc();
            _ = builder.Property(e => e.Duration).AsTicks();
            _ = builder.Ignore(e => e.ScheduleUniqueKey);

            _ = builder.HasMany(x => x.ProgrammeCategoryLinks).WithOne(x => x.Programme)
                .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasOne(e => e.ProgrammeDictionary).WithMany().HasForeignKey(e => e.ProgrammeDictionaryId);
            _ = builder.HasOne(e => e.Episode).WithMany().HasForeignKey(e => e.EpisodeId);
            _ = builder.HasOne(e => e.ScheduleProgramme).WithOne(x => x.Programme)
                .HasForeignKey<ScheduleProgramme>(x => x.ProgrammeId)
                .HasPrincipalKey<Programme>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasOne(e => e.SalesArea).WithMany().HasForeignKey(e => e.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(e => e.SalesAreaId);
            _ = builder.HasIndex(e => e.ProgrammeDictionaryId).Include<Programme>(e => e.LiveBroadcast);

            _ = builder.HasIndex(e => e.EpisodeId);
        }
    }
    public static class IndexExtension
    {
        public static IndexBuilder Include<TEntity>(this IndexBuilder indexBuilder, Expression<Func<TEntity, object>> indexExpression)
        {
            var includeStatement = new StringBuilder();
            foreach (var column in indexExpression.GetPropertyAccessList())
            {
                if (includeStatement.Length > 0)
                {
                    includeStatement.Append(", ");
                }
                includeStatement.AppendFormat("[{0}]", column.Name);
            }

            indexBuilder.HasAnnotation("SqlServer:IncludeIndex", includeStatement.ToString());

            return indexBuilder;
        }
    }
}
