using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Truncate;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Interfaces;
using Campaign = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.Campaign;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.CampaignCleaning
{
    public class CampaignCleaner : ICampaignCleaner
    {
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _sqlServerDbContextFactory;

        public CampaignCleaner(ISqlServerDbContextFactory<ISqlServerTenantDbContext> sqlServerDbContextFactory)
        {
            _sqlServerDbContextFactory = sqlServerDbContextFactory;
        }

        protected async Task InternalExecuteAsync(IReadOnlyCollection<Guid> campaignIds, CancellationToken cancellationToken)
        {
            using (var dbContext = _sqlServerDbContextFactory.Create())
            {
                foreach (var campaignId in campaignIds)
                {
                    dbContext.Specific.Attach(new Campaign { Id = campaignId }).State = EntityState.Deleted;
                }

                var prioritiesToRemove = await
                    (from scenarioCampaignPassPriority in dbContext.Query<ScenarioCampaignPassPriority>()
                     join scenario in dbContext.Query<Scenario>() on scenarioCampaignPassPriority.ScenarioId equals
                         scenario.Id
                     join compactCampaign in dbContext.Query<ScenarioCompactCampaign>() on
                         scenarioCampaignPassPriority.Id equals compactCampaign.ScenarioCampaignPassPriorityId
                     join runScenarioJoin in dbContext.Query<RunScenario>() on scenario.Id equals runScenarioJoin
                         .ScenarioId into rsJoin
                     from runScenario in rsJoin.DefaultIfEmpty()
                     where (scenario.IsLibraried == true || runScenario.Status == ScenarioStatus.Pending) &&
                           campaignIds.Contains(compactCampaign.Uid)
                     select new ScenarioCampaignPassPriority { Id = scenarioCampaignPassPriority.Id })
                    .AsNoTracking()
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var entity in prioritiesToRemove)
                {
                    dbContext.Specific.Attach(entity).State = EntityState.Deleted;
                }

                var runCampaignReferencesToRemove = await
                    (from runCampaignReference in dbContext.Query<RunCampaignReference>()
                     join campaign in dbContext.Query<Campaign>() on runCampaignReference.ExternalId equals campaign
                         .ExternalId
                     join run in dbContext.Query<Run>() on runCampaignReference.RunId equals run.Id
                     where run.RunStatus == RunStatus.NotStarted && campaignIds.Contains(campaign.Id)
                     select new RunCampaignReference { Id = runCampaignReference.Id })
                    .AsNoTracking()
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var entity in runCampaignReferencesToRemove)
                {
                    dbContext.Specific.Attach(entity).State = EntityState.Deleted;
                }

                var runCampaignProcessesSettingsToRemove = await
                        (from runCampaignProcessesSettings in dbContext.Query<RunCampaignProcessesSettings>()
                         join campaign in dbContext.Query<Campaign>() on runCampaignProcessesSettings.ExternalId equals campaign
                             .ExternalId
                         join run in dbContext.Query<Run>() on runCampaignProcessesSettings.RunId equals run.Id
                         where run.RunStatus == RunStatus.NotStarted && campaignIds.Contains(campaign.Id)
                         select new RunCampaignProcessesSettings { Id = runCampaignProcessesSettings.Id })
                        .AsNoTracking()
                        .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var entity in runCampaignProcessesSettingsToRemove)
                {
                    dbContext.Specific.Attach(entity).State = EntityState.Deleted;
                }

                await dbContext.SaveChangesWithConcurrencyConflictsResolvingAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using (var dbContext = _sqlServerDbContextFactory.Create())
            {
                await dbContext.Specific.TruncateOrDeleteAsync<Campaign>(
                        DeleteFromOptions.TruncateDependent | DeleteFromOptions.UseTransaction, cancellationToken)
                    .ConfigureAwait(false);

                var prioritiesToRemove = await
                    (from scenarioCampaignPassPriority in dbContext.Query<ScenarioCampaignPassPriority>()
                     join scenario in dbContext.Query<Scenario>() on scenarioCampaignPassPriority.ScenarioId equals
                         scenario.Id
                     join compactCampaign in dbContext.Query<ScenarioCompactCampaign>() on
                         scenarioCampaignPassPriority.Id equals compactCampaign.ScenarioCampaignPassPriorityId
                     join runScenarioJoin in dbContext.Query<RunScenario>() on scenario.Id equals runScenarioJoin
                         .ScenarioId into rsJoin
                     from runScenario in rsJoin.DefaultIfEmpty()
                     where scenario.IsLibraried == true || runScenario.Status == ScenarioStatus.Pending
                     select new ScenarioCampaignPassPriority { Id = scenarioCampaignPassPriority.Id })
                    .AsNoTracking()
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var entity in prioritiesToRemove)
                {
                    dbContext.Specific.Attach(entity).State = EntityState.Deleted;
                }

                var runCampaignReferencesToRemove = await
                    (from runCampaignReference in dbContext.Query<RunCampaignReference>()
                     join campaign in dbContext.Query<Campaign>() on runCampaignReference.ExternalId equals campaign
                         .ExternalId
                     join run in dbContext.Query<Run>() on runCampaignReference.RunId equals run.Id
                     where run.RunStatus == RunStatus.NotStarted
                     select new RunCampaignReference { Id = runCampaignReference.Id })
                    .AsNoTracking()
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var entity in runCampaignReferencesToRemove)
                {
                    dbContext.Specific.Attach(entity).State = EntityState.Deleted;
                }

                var runCampaignProcessesSettingsToRemove = await
                        (from runCampaignProcessesSettings in dbContext.Query<RunCampaignProcessesSettings>()
                         join campaign in dbContext.Query<Campaign>() on runCampaignProcessesSettings.ExternalId equals campaign
                             .ExternalId
                         join run in dbContext.Query<Run>() on runCampaignProcessesSettings.RunId equals run.Id
                         where run.RunStatus == RunStatus.NotStarted
                         select new RunCampaignProcessesSettings { Id = runCampaignProcessesSettings.Id })
                        .AsNoTracking()
                        .ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var entity in runCampaignProcessesSettingsToRemove)
                {
                    dbContext.Specific.Attach(entity).State = EntityState.Deleted;
                }

                await dbContext.SaveChangesWithConcurrencyConflictsResolvingAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public Task ExecuteAsync(string externalId, CancellationToken cancellationToken = default) =>
            ExecuteAsync(new[] { externalId }, cancellationToken);

        public async Task ExecuteAsync(IReadOnlyCollection<string> externalIds, CancellationToken cancellationToken = default)
        {
            if (externalIds is null || externalIds.Count == 0)
            {
                return;
            }

            IReadOnlyCollection<Guid> campaignIds;
            using (var dbContext = _sqlServerDbContextFactory.Create())
            {
                var ids = externalIds.Distinct().ToArray();
                campaignIds = await dbContext.Query<Campaign>().AsNoTracking()
                    .Where(x => ids.Contains(x.ExternalId))
                    .Select(x => x.Id)
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);
            }

            if (campaignIds?.Count > 0)
            {
                await InternalExecuteAsync(campaignIds, cancellationToken).ConfigureAwait(false);
            }
        }

        public Task ExecuteAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            return InternalExecuteAsync(new[] { campaignId }, cancellationToken);
        }

        public Task ExecuteAsync(IReadOnlyCollection<Guid> campaignIds, CancellationToken cancellationToken = default)
        {
            if (campaignIds is null || campaignIds.Count == 0)
            {
                return Task.CompletedTask;
            }

            return InternalExecuteAsync(campaignIds, cancellationToken);
        }

        public Task ExecuteAsync(IReadOnlyCollection<Domain.Campaigns.Objects.Campaign> campaigns,
            CancellationToken cancellationToken = default)
        {
            if (campaigns is null || campaigns.Count == 0)
            {
                return Task.CompletedTask;
            }

            return InternalExecuteAsync(campaigns.Select(x => x.Id).ToArray(), cancellationToken);
        }
    }
}
