using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;

namespace xggameplan.Updates
{
    /// <summary>
    /// Scenario V1, model before scenarios/passes were held in separate repositories.
    ///
    /// TODO: Remove this class when all environments are updated.
    /// </summary>
    public class ScenarioV1 : ICloneable
    {
        public virtual Guid Id { get; set; }

        public int CustomId { get; set; }

        public DateTime? StartedDateTime { get; set; }

        public DateTime? CompletedDateTime { get; set; }

        // public int CampaignPerformance { get; set; }

        // public int SpotPerformance { get; set; }

        //public double NewEfficiency { get; set; }

        public string Progress { get; set; }

        public virtual ScenarioStatuses Status { get; set; }

        public List<Pass> Passes = new List<Pass>();

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool IsScheduledOrRunning
        {
            get { return Status == ScenarioStatuses.Scheduled || Status == ScenarioStatuses.Starting || Status == ScenarioStatuses.Smoothing || Status == ScenarioStatuses.InProgress || Status == ScenarioStatuses.GettingResults; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool IsCompleted
        {
            get { return Status == ScenarioStatuses.CompletedSuccess || Status == ScenarioStatuses.CompletedError || Status == ScenarioStatuses.Deleted; }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
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
