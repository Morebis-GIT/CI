using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Utils.DataPurging.Attributes;
using ImagineCommunications.GamePlan.Utils.DataPurging.Exceptions;
using ImagineCommunications.GamePlan.Utils.DataPurging.Infrastructure;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using xggameplan.common.Extensions;
using xggameplan.core.Interfaces;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Handlers.Run
{
    /// <summary>
    /// Execute run purging according to the specified options
    /// </summary>
    [PurgingOptions("runs")]
    public class RunDataPurgingHandler : DataPurgingHandlerBase<RunPurgingOptions>
    {
        private readonly IRunCleaner _runCleaner;
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;
        private readonly ILogger<RunDataPurgingHandler> _logger;
        private readonly IClock _clock;

        public RunDataPurgingHandler(
            IOptionsSnapshot<RunPurgingOptions> options,
            IRunCleaner runCleaner,
            ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            ILogger<RunDataPurgingHandler> logger,
            IClock clock) : base(options)
        {
            _runCleaner = runCleaner;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _clock = clock;
        }

        protected ITargetBlock<Guid> CreateTargetBlock(IRunCleaner runCleaner, CancellationToken cancellationToken)
        {
            return new BatchExecuteTargetBlock<Guid>(ids => runCleaner.ExecuteAsync(ids, cancellationToken),
                Options.Concurrency, cancellationToken);
        }

        protected IQueryable<Persistence.SqlServer.Entities.Tenant.Runs.Run> CreatePurgingRunQuery(ISqlServerTenantDbContext dbContext)
        {
            return dbContext.Query<Persistence.SqlServer.Entities.Tenant.Runs.Run>()
                .Where(r => !r.Description.StartsWith("template "));
        }

        protected async Task<IReadOnlyCollection<Guid>> GetDaysAfterRunsAsync(CancellationToken cancellationToken)
        {
            var thresholdDate = _clock.GetCurrentInstant().ToDateTimeUtc().AddDays(-Options.DaysAfter);

            using (var dbContext = _dbContextFactory.Create())
            {
                var runIds = await CreatePurgingRunQuery(dbContext)
                    .Where(r => r.CreatedOrExecuteDateTime < thresholdDate)
                    .Select(r => r.Id)
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                _logger.LogDebug(
                    $"'{nameof(Options.DaysAfter)}' parameter is set to {Options.DaysAfter} days. {runIds.Length} run(s) returned which created/executed date less than {thresholdDate}.");

                return runIds;
            }
        }

        protected async Task<IReadOnlyCollection<Guid>> GetRunsAfterRunsAsync(CancellationToken cancellationToken)
        {
            using (var dbContext = _dbContextFactory.Create())
            {
                var query = CreatePurgingRunQuery(dbContext)
                    .Where(r => r.RunStatus == RunStatus.Complete || r.RunStatus == RunStatus.Errors);

                var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
                if (count > Options.RunsAfter)
                {
                    var runIds = await query
                        .OrderByDescending(x => x.ExecuteStartedDateTime)
                        .Skip(Options.RunsAfter)
                        .Select(x => x.Id)
                        .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                    _logger.LogDebug(
                        $"'{nameof(Options.RunsAfter)}' parameter is set to {Options.RunsAfter}. {runIds.Length} executed run(s) returned.");

                    return runIds;
                }

                _logger.LogDebug(
                    $"'{nameof(Options.RunsAfter)}' parameter is set to {Options.RunsAfter}. {count} executed run(s) is in database.");

                return Array.Empty<Guid>();
            }
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (Options.DaysAfter == 0 && Options.RunsAfter == 0)
            {
                _logger.LogInformation($"Configuration parameters hasn't been specified, execution skipped.");

                return;
            }

            if (Options.DaysAfter > 0)
            {
                var runIds = await GetDaysAfterRunsAsync(cancellationToken).ConfigureAwait(false);

                if (runIds.Count > 0)
                {
                    var targetBlock = CreateTargetBlock(_runCleaner, cancellationToken);
                    foreach (var runId in runIds)
                    {
                        _ = await targetBlock.SendAsync(runId, cancellationToken).ConfigureAwait(false);
                    }

                    targetBlock.Complete();
                    await targetBlock.Completion.AggregateExceptions<DataPurgingException>(
                            "Purging of Run data has been failed. See inner exception for more details.")
                        .ConfigureAwait(false);

                    _logger.LogDebug(
                        $"Purging of {runIds.Count} run(s) has been completed successfully according to the '{nameof(Options.DaysAfter)}' parameter.");
                }
            }
            else
            {
                _logger.LogDebug(
                    $"'{nameof(Options.DaysAfter)}' configuration parameters hasn't been specified, execution skipped.");
            }

            if (Options.RunsAfter > 0)
            {
                var runIds = await GetRunsAfterRunsAsync(cancellationToken).ConfigureAwait(false);
                if (runIds.Count > 0)
                {
                    var targetBlock = CreateTargetBlock(_runCleaner, cancellationToken);
                    foreach (var runId in runIds)
                    {
                        _ = await targetBlock.SendAsync(runId, cancellationToken).ConfigureAwait(false);
                    }

                    targetBlock.Complete();
                    await targetBlock.Completion.AggregateExceptions<DataPurgingException>(
                            "Purging of Run data has been failed. See inner exception for more details.")
                        .ConfigureAwait(false);

                    _logger.LogDebug(
                        $"Purging of {runIds.Count} run(s) has been completed successfully according to the '{nameof(Options.RunsAfter)}' parameter.");
                }
            }
            else
            {
                _logger.LogDebug(
                    $"'{nameof(Options.RunsAfter)}' configuration parameters hasn't been specified, execution skipped.");
            }
        }
    }
}
