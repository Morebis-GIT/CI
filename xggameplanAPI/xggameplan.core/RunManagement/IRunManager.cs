using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.Model;

namespace xggameplan.RunManagement
{
    /// <summary>
    /// Interface for managing runs
    /// </summary>
    public interface IRunManager
    {
        /// <summary>
        /// Starts run, all scenarios
        /// </summary>
        /// <param name="runId"></param>
        /// <returns>One RunInstance per scenario started</returns>
        List<RunInstance> AllScenariosStartRun(Guid runId);

        /// <summary>
        /// Starts run of next scenario on specific AutoBook. This is typically done when an AutoBook instance has provisioned
        /// </summary>
        /// <param name="autoBook"></param>
        /// <returns>RunInstance is started, null=No scenario to start</returns>
        RunInstance NextScheduledScenarioStartRun(AutoBook autoBook);

        /// <summary>
        /// Deletes run and all scenarios
        /// </summary>
        /// <param name="runId"></param>
        void DeleteRun(Guid runId);

        /// <summary>
        /// Validates that the run can start, throws exception if it can't
        /// </summary>
        /// <param name="">run</param>
        List<SystemMessage> ValidateForStartRun(Run run);

        /// <summary>
        /// Creates input files for testing
        /// </summary>
        /// <param name="run"></param>
        void CreateInputFilesForTest(Run run);

        /// <summary>
        /// Processes spots output file for testing
        /// </summary>
        /// <param name="run"></param>
        /// <param name="testFileFolder"></param>
        void ProcessSpotsOutputFileForTest(Run run, string testFileFolder);

        /// <summary>
        /// Handles crashed runs
        /// </summary>
        /// <returns></returns>
        List<Run> HandleCrashedRuns();

        /// <summary>
        /// Handles call to external inventory system to request a lock
        /// </summary>
        /// <param name="run"></param>
        bool InventoryLock(Run run);

        /// <summary>
        /// Handles call to external inventory system to request an unlock
        /// </summary>
        /// <param name="run"></param>
        bool InventoryUnlock(Run run, Guid? scenarioId);

        bool Exists(Guid runId);

        void ApplyCampaignProcessesConfigurations(
            IEnumerable<CampaignRunProcessesSettings> campaignRunProcessesSettings, Guid? runId = null);

        void CreateNotificationForCompletedRun(Run run);

        void BroadcastScenario(Guid id, ScenarioStatuses status, int currentStep, int totalSteps);
    }
}
