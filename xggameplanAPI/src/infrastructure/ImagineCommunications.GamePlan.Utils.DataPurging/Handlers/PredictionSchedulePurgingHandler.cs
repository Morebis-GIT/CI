using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using ImagineCommunications.GamePlan.Utils.DataPurging.Attributes;
using ImagineCommunications.GamePlan.Utils.DataPurging.Exceptions;
using ImagineCommunications.GamePlan.Utils.DataPurging.Infrastructure;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using xggameplan.common.Extensions;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Handlers
{
    /// <inheritdoc />
    [PurgingOptions("predictionSchedules")]
    public class PredictionSchedulePurgingHandler : DataPurgingHandlerBase<PredictionSchedulesPurgingOptions>
    {
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;
        private readonly IClock _clock;
        private readonly ILogger<PredictionSchedulePurgingHandler> _logger;

        public PredictionSchedulePurgingHandler(ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            IClock clock,
            IOptionsSnapshot<PredictionSchedulesPurgingOptions> options,
            ILogger<PredictionSchedulePurgingHandler> logger) : base(options)
        {
            _dbContextFactory = dbContextFactory;
            _clock = clock;
            _logger = logger;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (Options.DaysAfter <= 0)
            {
                _logger.LogInformation($"'{nameof(Options.DaysAfter)}' parameters hasn't been specified, PredictionSchedules purging skipped.");
                return;
            }

            var thresholdDate = _clock.GetCurrentInstant().ToDateTimeUtc().AddDays(-Options.DaysAfter).Date;

            _logger.LogDebug($"'{nameof(Options.DaysAfter)}' parameter is set to {Options.DaysAfter}.");
            _logger.LogDebug($"Threshold DateTime is {thresholdDate}.");

            await BatchDeleteEntitiesAsync<PredictionSchedule>(x => x.ScheduleDay.Date < thresholdDate, cancellationToken)
                .ConfigureAwait(false);

        }

        private async Task BatchDeleteEntitiesAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken) where TEntity : class, IIdentityPrimaryKey, new()
        {
            using (var dbContext = _dbContextFactory.Create())
            {
                var entities = await dbContext
                    .Query<TEntity>()
                    .AsNoTracking()
                    .Where(predicate)
                    .Select(x => new TEntity { Id = x.Id })
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                _logger.LogDebug($"{entities.Length} {typeof(TEntity).Name}(s) are ready to purge.");

                var batchDeleteBlock = new BatchExecuteTargetBlock<TEntity>(
                    elements => RemoveEntitiesInternalAsync(elements, cancellationToken), Options.Concurrency,
                    cancellationToken);

                foreach (var entity in entities)
                {
                    _ = await batchDeleteBlock.SendAsync(entity, cancellationToken).ConfigureAwait(false);
                }

                batchDeleteBlock.Complete();

                await batchDeleteBlock.Completion.AggregateExceptions<DataPurgingException>(
                    "Purging of PredictionSchedules data has been failed. See inner exception for more details.");

                _logger.LogDebug(
                    $"Purging of {entities.Length} {typeof(TEntity).Name}(s) has been completed successfully according to the '{nameof(Options.DaysAfter)}' parameter.");
            }
        }

        private async Task RemoveEntitiesInternalAsync<TEntity>(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken) where TEntity : class
        {
            using (var dbContext = _dbContextFactory.Create())
            {
                foreach (var entity in entities)
                {
                    dbContext.Specific.Attach(entity).State = EntityState.Deleted;
                }

                await dbContext.SaveChangesWithConcurrencyConflictsResolvingAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
