using System;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Raven.Imports.Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    /// <summary>
    /// Scenario selected for a run
    /// </summary>
    public class RunScenario : ICloneable
    {
        public virtual Guid Id { get; set; }

        public DateTime? StartedDateTime { get; set; }

        public DateTime? CompletedDateTime { get; set; }

        public string Progress { get; set; }

        public virtual ScenarioStatuses Status { get; set; }

        public ExternalRunInfo ExternalRunInfo { get; set; }


        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateUserModified { get; set; }
        public int TotalSteps { get; set; }
        public int CurrentStep { get; set; }

        public Guid ScenarioId { get; set; }

        public int Order { get; set; }

        [JsonIgnore]
        public bool IsScheduledOrRunning
        {
            get
            {
                return
                    Status == ScenarioStatuses.Scheduled ||
                    Status == ScenarioStatuses.Starting ||
                    Status == ScenarioStatuses.Smoothing ||
                    Status == ScenarioStatuses.InProgress ||
                    Status == ScenarioStatuses.GettingResults;
            }
        }

        [JsonIgnore]
        public bool IsCompleted
        {
            get
            {
                return
                    Status == ScenarioStatuses.CompletedSuccess ||
                    Status == ScenarioStatuses.CompletedError ||
                    Status == ScenarioStatuses.Deleted;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Resets the scenario to initial Pending status
        /// </summary>
        public void ResetToPendingStatus()
        {
            Status = ScenarioStatuses.Pending;
            Progress = "DRAFT";
            StartedDateTime = null;
            CompletedDateTime = null;
        }

        public static void ValidateBeforeSetDefault(Scenario scenario)
        {
            if (scenario.Passes.Count == 0)
            {
                throw new Exception("Must have at least one pass");
            }
        }
    }
}
