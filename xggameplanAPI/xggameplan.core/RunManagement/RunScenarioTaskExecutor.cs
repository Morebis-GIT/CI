using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.RunManagement;

namespace xggameplan.core.RunManagement
{
    public class RunScenarioTaskExecutor : IDisposable
    {
        private readonly ILifetimeScope _lifetimeScope;
        // Limit number of scenarios that can be simultaneously started
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3);

        public RunScenarioTaskExecutor(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public Task Execute(Run run,
            RunScenario scenario,
            IReadOnlyCollection<AutoBookInstanceConfiguration> autoBookInstanceConfigurationsForRun,
            double autoBookRequiredStorageGB,
            ConcurrentBag<RunInstance> runInstances,
            ConcurrentDictionary<Guid, ScenarioStatuses> newScenarioStatuses,
            ConcurrentDictionary<Guid, bool> scenarioSyncStatuses,
            bool autoDistributed)
        {
            return Task.Run(() =>
            {
                _semaphore.Wait();
                try
                {
                    using var scope = _lifetimeScope.BeginLifetimeScope();
                    var scenarioTask = scope.Resolve<RunScenarioTask>();
                    scenarioTask.Execute(run, scenario, autoBookInstanceConfigurationsForRun, autoBookRequiredStorageGB,
                        runInstances, newScenarioStatuses, scenarioSyncStatuses, autoDistributed);
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        public void Dispose() => _semaphore?.Dispose();
    }
}
