using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookTaskReports;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PipelineAuditEvents;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.RunCleaning
{
    /// <summary>
    /// Represents a collection of <see cref="RemovableEntitiesCollectHandlerInfo"/> instances
    /// to enumerate it while entities deletion.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.RunCleaning.RemovableEntitiesCollectHandlerInfo}" />
    internal class RemovableEntitiesCollectHandlerCollection : IEnumerable<RemovableEntitiesCollectHandlerInfo>
    {
        private static readonly string[] _cleanProcessors = { "smooth", "autobook", "isr", "rzr" };

        private readonly IReadOnlyCollection<Guid> _runIds;
        private readonly IReadOnlyCollection<Guid> _scenarioIds;
        private readonly CancellationToken _cancellationToken;

        public RemovableEntitiesCollectHandlerCollection(
            IReadOnlyCollection<Guid> runIds,
            IReadOnlyCollection<Guid> scenarioIds,
            CancellationToken cancellationToken)
        {
            _runIds = runIds;
            _scenarioIds = scenarioIds;
            _cancellationToken = cancellationToken;
        }

        public Task<IReadOnlyCollection<object>> CollectAutobookFailures(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<Failure>(dbContext,
                x => _scenarioIds.Contains(x.ScenarioId), _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectRecommendationProcessors(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<Recommendation>(dbContext,
                x => _scenarioIds.Contains(x.ScenarioId) && _cleanProcessors.Contains(x.Processor),
                _cancellationToken);
        }

        public async Task<IReadOnlyCollection<object>> CollectResultFiles(ISqlServerTenantDbContext dbContext)
        {
            return await (from outputFile in dbContext.Query<OutputFile>()
                          join resultFile in dbContext.Query<ResultsFile>()
                              .Where(x => _scenarioIds.Contains(x.ScenarioId)) on outputFile.FileId equals resultFile
                              .FileId
                          select new ResultsFile { Id = resultFile.Id }).AsNoTracking().ToArrayAsync(_cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<IReadOnlyCollection<object>> CollectSmoothFailures(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<SmoothFailure>(dbContext,
                x => _runIds.Contains(x.RunId), _cancellationToken);
        }

        public async Task<IReadOnlyCollection<object>> CollectScenarioPasses(ISqlServerTenantDbContext dbContext)
        {
            Pass[] entities = await (
                    from scenarioPassReference in dbContext.Query<ScenarioPassReference>()
                        .Where(x => _scenarioIds.Contains(x.ScenarioId))
                    join pass in dbContext.Query<Pass>() on scenarioPassReference.PassId equals pass.Id
                    select new Pass { Id = pass.Id }).AsNoTracking().ToArrayAsync(_cancellationToken)
                .ConfigureAwait(false);

            List<Pass> passesToDelete = new List<Pass>();

            foreach (Pass pass in entities)
            {
                var toDelete = dbContext.Query<Pass>()
                    .Include(x => x.RatingPoints)
                        .ThenInclude(x => x.SalesAreas)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == pass.Id);

                passesToDelete.Add(toDelete);
            }

            return passesToDelete;
        }

        public Task<IReadOnlyCollection<object>> CollectScenarioCampaignResult(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<ScenarioCampaignResult>(dbContext,
                x => _scenarioIds.Contains(x.ScenarioId), _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectScenarioCampaignFailures(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<ScenarioCampaignFailure>(dbContext,
                x => _scenarioIds.Contains(x.ScenarioId), _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectScenarioCampaignMetrics(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<ScenarioCampaignMetric>(dbContext,
                x => _scenarioIds.Contains(x.ScenarioId), _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectAutobookTasks(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<AutoBookTask>(dbContext,
                x => _scenarioIds.Contains(x.ScenarioId), _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectPipelineAuditEvents(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<PipelineAuditEvent>(dbContext,
                x => _scenarioIds.Contains(x.ScenarioId), _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectAutobookTaskReports(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<AutoBookTaskReport>(dbContext,
                x => _scenarioIds.Contains(x.ScenarioId), _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectScenarioResults(ISqlServerTenantDbContext dbContext)
        {
            return CollectIdentityEntitiesToRemove<ScenarioResult>(dbContext, x => _scenarioIds.Contains(x.ScenarioId),
                _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectScenarios(ISqlServerTenantDbContext dbContext)
        {
            return CollectGuidEntitiesToRemove<Scenario>(dbContext, x => _scenarioIds.Contains(x.Id),
                _cancellationToken);
        }

        public Task<IReadOnlyCollection<object>> CollectRuns(ISqlServerTenantDbContext dbContext)
        {
            return CollectGuidEntitiesToRemove<Run>(dbContext, x => _runIds.Contains(x.Id),
                _cancellationToken);
        }

        private async Task<IReadOnlyCollection<object>> CollectIdentityEntitiesToRemove<TEntity>(
            ISqlServerTenantDbContext dbContext,
            Expression<Func<TEntity, bool>> wherePredicate, CancellationToken cancellationToken)
            where TEntity : class, IIdentityPrimaryKey, new()
        {
            return await dbContext.Query<TEntity>().AsNoTracking()
                .Where(wherePredicate)
                .Select(x => new TEntity { Id = x.Id })
                .ToArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task<IReadOnlyCollection<object>> CollectGuidEntitiesToRemove<TEntity>(
            ISqlServerTenantDbContext dbContext,
            Expression<Func<TEntity, bool>> wherePredicate, CancellationToken cancellationToken)
            where TEntity : class, IUniqueIdentifierPrimaryKey, new()
        {
            return await dbContext.Query<TEntity>().AsNoTracking()
                .Where(wherePredicate)
                .Select(x => new TEntity { Id = x.Id })
                .ToArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        public IEnumerator<RemovableEntitiesCollectHandlerInfo> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Enumerator : IEnumerator<RemovableEntitiesCollectHandlerInfo>
        {
            private readonly RemovableEntitiesCollectHandlerInfo[] _delegateInfo;
            private int _position = -1;

            public Enumerator(RemovableEntitiesCollectHandlerCollection collection)
            {
                _delegateInfo = new[]
                {
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(Failure), collection.CollectAutobookFailures),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(Recommendation), collection.CollectRecommendationProcessors),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(ResultsFile), collection.CollectResultFiles),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(SmoothFailure), collection.CollectSmoothFailures),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(Pass), collection.CollectScenarioPasses),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(ScenarioCampaignResult), collection.CollectScenarioCampaignResult),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(ScenarioCampaignFailure), collection.CollectScenarioCampaignFailures),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(ScenarioCampaignMetric), collection.CollectScenarioCampaignMetrics),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(AutoBookTask), collection.CollectAutobookTasks),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(PipelineAuditEvent), collection.CollectPipelineAuditEvents),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(AutoBookTaskReport), collection.CollectAutobookTaskReports),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(ScenarioResult), collection.CollectScenarioResults),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(Scenario), collection.CollectScenarios),
                    RemovableEntitiesCollectHandlerInfo.Create(nameof(Run), collection.CollectRuns)
                };
            }

            public bool MoveNext()
            {
                _position++;

                return _position < _delegateInfo.Length;
            }

            public void Reset() => _position = -1;

            public RemovableEntitiesCollectHandlerInfo Current => _delegateInfo[_position];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}
