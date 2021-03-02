using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.RunManagement.Notifications;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests run notification (E.g. Mulesoft)
    /// </summary>
    internal class RunNotificationTest : ISystemTest
    {
        private readonly IRepositoryFactory _repositoryFactory;        
        private const string _category = "Run Notifications";
        
        public RunNotificationTest(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;            
        }

        public bool CanExecute(SystemTestCategories systemTestCategory)
        {
            return (systemTestCategory == SystemTestCategories.Deployment); // Only relevant for installation test
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            List<SystemTestResult> results = new List<SystemTestResult>();

            try
            {
                // Execute run notification HTTP tests
                results.AddRange(ExecuteRunNotificationHTTPTests());
            }
            catch(System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error checking run notification: {0}", exception.Message), ""));
            }
            return results;
        }

        /// <summary>
        /// Execute run notification HTTP tests
        /// </summary>
        /// <returns></returns>
        private List<SystemTestResult> ExecuteRunNotificationHTTPTests()
        {
            var results = new List<SystemTestResult>();

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var tenantSettingsRepository = scope.CreateRepository<ITenantSettingsRepository>();
                var tenantSettings = tenantSettingsRepository.Get();

                // Find RunEventSettings for the scenario completed event
                var runEventSettings = tenantSettings.RunEventSettings.Find(res => res.EventType == RunEvents.RunScenarioCompleted);
                if (runEventSettings != null && runEventSettings.HTTP != null)   // HTTP notification is configured
                {
                    try
                    {
                        // Create a run in memory with ScenarioIds that won't exist in external system
                        var runRepository = scope.CreateRepository<IRunRepository>();
                        var templateRun = runRepository.GetAll().Where(r => r.Scenarios.Any()).FirstOrDefault();
                        if (templateRun == null)   // No existing runs
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, "Unable to test HTTP notification for run scenario completed because there is no run to use. Please check that the notification is configured correctly otherwise output data will not be picked up and processed by the external system.", ""));
                        }
                        else   // Existing run, clone it
                        {
                            var run = (Run)templateRun.Clone();
                            run.Id = Guid.NewGuid();
                            run.Scenarios.ForEach(scenario => scenario.Id = Guid.NewGuid());

                            // Execute notification
                            HTTPTNotification httpNotification = new HTTPTNotification();
                            httpNotification.RunCompleted(run, run.Scenarios[0], true, runEventSettings.HTTP);
                        }
                    }
                    catch (System.Exception exception)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error calling HTTP notification for run scenario completed: {0}. The output data for runs will never be picked up and processed by the external system.", exception.Message), ""));
                    }
                }
                else    // Notification not set up
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "There is no HTTP notification configured for when a scenario run is completed. The output data for runs will never be picked up and processed by the external system.", ""));
                }
            }
            return results;
        }
    }
}
