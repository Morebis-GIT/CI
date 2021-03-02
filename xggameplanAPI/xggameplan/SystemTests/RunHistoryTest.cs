using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests run history. Reports of runs with issues
    /// </summary>
    internal class RunHistoryTest : ISystemTest
    {
        private IRepositoryFactory _repositoryFactory;
        private const string _category = "Run History";

        public RunHistoryTest(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories)
        {
            return (systemTestCategories != SystemTestCategories.Deployment);    // Run history isn't relevant for installation test
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();
            int hoursSinceRunStarted = 24;
            int scenarioCompleteTimeoutMins = 120;

            try
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var runRepository = scope.CreateRepository<IRunRepository>();

                    // Get any runs started in last N hours
                    int countFailedRuns = 0;
                    var runs = runRepository.GetAll().Where(r => r.FirstScenarioStartedDateTime != null && r.FirstScenarioStartedDateTime >= DateTime.UtcNow.AddHours(-hoursSinceRunStarted)).OrderBy(r => r.CreatedDateTime);

                    // Check runs
                    foreach (var run in runs)
                    {
                        // Count up failures
                        countFailedRuns += run.Scenarios.Where(s => s.Status == ScenarioStatuses.CompletedError).ToList().Count;

                        try
                        {
                            ISystemTest runOutputTest = new RunOutputTest(run, _category, _repositoryFactory, scenarioCompleteTimeoutMins);
                            results.AddRange(runOutputTest.Execute(systemTestCategory));                            
                        }
                        catch { };      // Ignore
                    }

                    if (!runs.Any())
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("No runs were started in the last {0} hours", hoursSinceRunStarted), ""));
                    }
                    else if (countFailedRuns == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("No failed runs in the last {0} hours", hoursSinceRunStarted), ""));
                    }
                }
            }
            catch(System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error checking run history: {0}", exception.Message), ""));
            }
            return results;
        }

        //private List<AuditEvent> GetRunAuditEvents(Guid runId, Guid scenarioId)
        //{
        //    // Create filter for pipeline events for scenario
        //    AuditEventFilter filter = new AuditEventFilter()
        //    {
        //        IncludeValues = true,
        //        AllFiltersRequired = true
        //    };
        //    filter.EventTypeIds.AddRange(new int[] { AuditEventTypes.xGG_AutoBook, AuditEventTypes.xGG_GameplanRun, AuditEventTypes.xGG_SmoothRun });
        //    filter.ValueFilters.Add(new AuditEventValueFilter() { ValueTypeID = AuditEventValueTypes.xGG_ScenarioID, Value = scenarioId });

        //    // Get audit events
        //    var auditEvents = _auditEventRepository.Get(filter).OrderBy(ae => ae.TimeCreated).ToList();
        //    return auditEvents;
        //}
    }
}
