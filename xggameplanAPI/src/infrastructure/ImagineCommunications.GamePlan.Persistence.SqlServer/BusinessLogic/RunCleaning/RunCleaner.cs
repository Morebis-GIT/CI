using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using xggameplan.cloudaccess.Interfaces;
using xggameplan.common.Extensions;
using xggameplan.core.Services.RunCleaning;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.RunCleaning
{
    /// <summary>
    /// Exposes functionality to delete runs from SqlServer database.
    /// </summary>
    /// <seealso cref="xggameplan.core.Services.RunCleaning.RunCleanerBase" />
    public class RunCleaner : RunCleanerBase
    {
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;

        public RunCleaner(ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            ICloudStorageV2 cloudStorage) : base(cloudStorage)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override async Task<IReadOnlyCollection<RunDeletionInfo>> GetRunDeletionInfoAsync(
            IReadOnlyCollection<Guid> runIds,
            CancellationToken cancellationToken)
        {
            using (var dbContext = _dbContextFactory.Create())
            {
                return await dbContext.Query<Run>().AsNoTracking().Include(x => x.Scenarios)
                    .Where(r => runIds.Contains(r.Id))
                    .Select(r => new RunDeletionInfo
                    {
                        RunId = r.Id,
                        ScenarioIds = r.Scenarios.Select(s => s.ScenarioId).ToArray()
                    }).ToArrayAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        protected override async Task DeleteRunDataAsync(IReadOnlyCollection<RunDeletionInfo> runDeletionInfos,
            CancellationToken cancellationToken)
        {
            var allRunIds = runDeletionInfos.Select(r => r.RunId).ToArray();
            var allScenarioIds = runDeletionInfos.SelectMany(r => r.ScenarioIds).Distinct().ToArray();

            var actionBlock =
                new ActionBlock<RemovableEntitiesCollectHandlerInfo>(RemoveEntitiesAsync,
                    new ExecutionDataflowBlockOptions
                    {
                        CancellationToken = cancellationToken,
                        MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
                    });

            var collectHandlerCollection =
                new RemovableEntitiesCollectHandlerCollection(allRunIds, allScenarioIds, cancellationToken);

            foreach (var itemInfo in collectHandlerCollection.Where(x => x.EntityName != nameof(Run)))
            {
                _ = await actionBlock.SendAsync(itemInfo, cancellationToken).ConfigureAwait(false);
            }

            actionBlock.Complete();

            await actionBlock.Completion.AggregateExceptions().ConfigureAwait(false);

            // removes Run entities only when all dependent entities have been removed successfully
            var runCollectHandlerInfo = collectHandlerCollection.FirstOrDefault(x => x.EntityName == nameof(Run));
            if (!(runCollectHandlerInfo is null))
            {
                await RemoveEntitiesAsync(runCollectHandlerInfo).ConfigureAwait(false);
            }

            #region Local functions

            async Task RemoveEntitiesAsync(RemovableEntitiesCollectHandlerInfo handlerInfo)
            {
                try
                {
                    using (var dbContext = _dbContextFactory.Create())
                    {
                        var entities = await handlerInfo.CollectFunc(dbContext).ConfigureAwait(false);

                        foreach (var entity in entities)
                        {
                            dbContext.Specific.Attach(entity).State = EntityState.Deleted;
                        }

                        while (true)
                        {
                            try
                            {
                                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                                break;
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {
                                if (ex.Entries.Count == 0)
                                {
                                    throw;
                                }

                                foreach (var entry in ex.Entries)
                                {
                                    entry.State = EntityState.Detached;
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var formatterRunInfo = allRunIds.Length == 1
                        ? $"run with id '{allRunIds.First()}'"
                        : $"{allRunIds.Length} runs ({string.Join(", ", allRunIds.Select(x => $"'{x}'"))})";

                    throw new RunCleaningException(
                        $"Deletion of {formatterRunInfo} has been failed while removing '{handlerInfo.EntityName}' entities. See inner exception for more details.",
                        ex);
                }
            }

            #endregion Local functions
        }
    }
}
