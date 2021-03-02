using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using Microsoft.Extensions.Logging;
using xggameplan.common.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator
{
    public class BreakAvailabilityUpdater
    {
        private readonly ILogger _logger;
        private readonly ISqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext> _tenantDbContextFactory;
        private readonly RecalculateBreakAvailabilityOptions _recalculateOptions;
        private readonly CancellationToken _cancellationToken;

        private ActionBlock<IBreakAvailability[]> _internalBlock;
        private int _batchId;

        protected async Task SaveBreaksAsync(IBreakAvailability[] breaks)
        {
            if (breaks.Length == 0)
            {
                return;
            }

            var batchId = Interlocked.Increment(ref _batchId);
            _logger.LogInformation($"(BatchId: {batchId.ToString()}): Updating of {breaks.Length.ToString()} breaks started.");
            var hasErrors = false;

            try
            {
                using var dbContext = _tenantDbContextFactory.Create();

                var breakEntities = breaks.Select(x => new Break
                {
                    Id = x.Id,
                    Avail = x.Avail.ToTimeSpan(),
                    OptimizerAvail = x.OptimizerAvail.ToTimeSpan()
                }).ToList();

                var scheduleBreakEntities = breaks.Select(x => new ScheduleBreak
                {
                    Id = x.Id,
                    Avail = x.Avail.ToTimeSpan(),
                    OptimizerAvail = x.OptimizerAvail.ToTimeSpan()
                }).ToList();

                var bulkInsertOptions = new BulkInsertOptions
                {
                    BatchSize = breaks.Length,
                    PropertiesToInclude = new List<string>
                    {
                        nameof(Break.Avail),
                        nameof(Break.OptimizerAvail)
                    }
                };

                using var tran = await dbContext.Specific.Database.BeginTransactionAsync(_cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.BulkInsertEngine
                    .BulkUpdateAsync(breakEntities, bulkInsertOptions, _cancellationToken)
                    .ConfigureAwait(false);
                await dbContext.BulkInsertEngine
                    .BulkUpdateAsync(scheduleBreakEntities, bulkInsertOptions, _cancellationToken)
                    .ConfigureAwait(false);

                tran.Commit();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"(BatchId: {batchId.ToString()}): Updating of {breaks.Length.ToString()} breaks cancelled.");
                hasErrors = true;
            }
            catch
            {
                _logger.LogError($"(BatchId: {batchId.ToString()}): Updating of {breaks.Length.ToString()} breaks finished with errors.");
                hasErrors = true;
                throw;
            }
            finally
            {
                if (!hasErrors)
                {
                    _logger.LogInformation($"(BatchId: {batchId.ToString()}): Updating of {breaks.Length.ToString()} breaks finished successfully.");
                }
            }
        }

        public BreakAvailabilityUpdater(
            ILogger logger,
            ISqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext> tenantDbContextFactory,
            RecalculateBreakAvailabilityOptions recalculateOptions,
            CancellationToken cancellationToken)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenantDbContextFactory = tenantDbContextFactory ?? throw new ArgumentNullException(nameof(tenantDbContextFactory));
            _recalculateOptions = recalculateOptions ?? throw new ArgumentNullException(nameof(recalculateOptions));
            _cancellationToken = cancellationToken;
        }

        public bool Start(ISourceBlock<IBreakAvailability[]> sourceBlock, Action faultAction = null)
        {
            if (sourceBlock is null)
            {
                throw new ArgumentNullException(nameof(sourceBlock));
            }

            if (!(_internalBlock is null))
            {
                return false;
            }

            _internalBlock =
                new ActionBlock<IBreakAvailability[]>(
                    SaveBreaksAsync,
                    new ExecutionDataflowBlockOptions
                    {
                        CancellationToken = _cancellationToken,
                        MaxDegreeOfParallelism = _recalculateOptions.UpdateBreakDegreeOfParallelism
                    });

            _ = sourceBlock.LinkTo(_internalBlock);
            _ = sourceBlock.Completion.ContinueWith(_ => _internalBlock.Complete());

            if (faultAction is null)
            {
                return true;
            }

            _ = _internalBlock.Completion.ContinueWith(_ =>
                faultAction(),
                TaskContinuationOptions.OnlyOnFaulted);

            return true;
        }

        public async Task WaitAsync()
        {
            if (_internalBlock is null)
            {
                return;
            }

            var exceptions = await _internalBlock.Completion
                .WaitWithTaskExceptionGatheringAsync()
                .ConfigureAwait(false);

            if (exceptions.Count > 0)
            {
                await Task.FromException(new AggregateException(exceptions)).ConfigureAwait(false);
            }
        }
    }
}
