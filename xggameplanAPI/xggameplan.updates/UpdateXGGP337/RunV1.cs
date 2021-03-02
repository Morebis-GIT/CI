using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.Model;

namespace xggameplan.Updates
{
    /// <summary>
    /// Run V1, model before scenarios/passes were held in separate repositories.
    ///
    /// TODO: Remove this class when all environments are updated.
    /// </summary>
    public class RunV1 : ICloneable
    {
        public virtual Guid Id { get; set; }

        public int CustomId { get; set; }
        public string Description { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public DateTime? ExecuteStartedDateTime { get; set; }

        public bool IsLocked { get; set; }

        public bool Real { get; set; }

        public bool Smooth { get; set; }

        public bool ISR { get; set; }

        public bool Optimisation { get; set; }

        public bool RightSizer { get; set; }

        public AuthorModel Author { get; set; }

        //public List<SalesArea> SalesAreas = new List<SalesArea>();
        public List<SalesAreaReference> SalesAreas { get; set; } = new List<SalesAreaReference>();

        //public List<Campaign> Campaigns = new List<Campaign>();
        public List<CampaignReference> Campaigns { get; set; } = new List<CampaignReference>();

        public virtual List<ScenarioV1> Scenarios { get; set; } = new List<ScenarioV1>();
        //public List<ScenarioReference> Scenarios = new List<ScenarioReference>();

        public RunV1()
        {
            Optimisation = true;    // Default
            RightSizer = false;     // Default
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool IsCompleted
        {
            get { return Scenarios.Where(scenario => scenario.IsCompleted).Count() == Scenarios.Count(); }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public List<ScenarioV1> ScheduledScenarios
        {
            get { return Scenarios.Where(scenario => scenario.Status == ScenarioStatuses.Scheduled).ToList(); }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public List<ScenarioV1> CompletedScenarios
        {
            get { return Scenarios.Where(scenario => scenario.IsCompleted).ToList(); }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public DateTime? FirstScenarioStartedDateTime
        {
            get { return this.Scenarios.Where(s => s.StartedDateTime != null).OrderBy(s => s.StartedDateTime).FirstOrDefault()?.StartedDateTime; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public DateTime? LastScenarioCompletedDateTime
        {
            get { return this.Scenarios.Where(s => s.IsCompleted && s.CompletedDateTime != null).OrderBy(s => s.CompletedDateTime).LastOrDefault()?.CompletedDateTime; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool IsTemplate
        {
            get { return this.Description.ToLower().StartsWith("template "); }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Validates run for saving. Does not have to be valid for run at this stage.
        /// </summary>
        /// <param name="run"></param>
        public static void ValidateForSave(RunV1 run)
        {
            if (String.IsNullOrEmpty(run.Description))
            {
                throw new Exception("Scenario description must be set");
            }

            List<Guid> scenarioIds = new List<Guid>();
            foreach (var scenario in run.Scenarios)
            {
                if (scenarioIds.Contains(scenario.Id))
                {
                    throw new Exception("Scenario ID must be unique");
                }

                if (scenario.Passes.Count == 0)
                {
                    throw new Exception("Scenario must have at least one pass");
                }

                if (scenario.IsScheduledOrRunning)
                {
                    throw new Exception("Cannot update scenario while it is running or scheduled to run");
                }

                if (scenario.IsCompleted)
                {
                    throw new Exception("Cannot update scenario because it has completed");
                }
                scenarioIds.Add(scenario.Id);
            }
        }

        /// <summary>
        /// Validates for deleting
        /// </summary>
        /// <param name="run"></param>

        public static void ValidateForDelete(RunV1 run)
        {
            if (run.Scenarios.Where(x => x.IsScheduledOrRunning).FirstOrDefault() != null)
            {
                throw new Exception("Cannot delete while scenarios are running or scheduled to run");
            }
            if (run.IsTemplate)
            {
                throw new Exception("Not allowed to delete the run");
            }
        }

        /// <summary>
        /// Validates for deleting all scenarios
        /// </summary>
        /// <param name="run"></param>
        public static void ValidateForDeleteScenarios(RunV1 run)
        {
            if (run.Scenarios.Where(x => x.IsScheduledOrRunning).FirstOrDefault() != null)
            {
                throw new Exception("Cannot delete scenarios that are running or scheduled to run");
            }
        }

        /// <summary>
        /// Validates for deleting scenario
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenario"></param>
        public static void ValidateForDeleteScenario(Run runV1, ScenarioV1 scenario)
        {
            if (scenario.IsScheduledOrRunning)
            {
                throw new Exception("Cannot delete a scenario that is running or scheduled to run");
            }
        }

        /// <summary>
        /// Validates for run
        /// </summary>
        /// <param name="run"></param>
        public static void ValidateForRun(RunV1 run)
        {
            int completedScenarios = run.Scenarios.Where(x => x.IsCompleted).ToList().Count;
            int pendingScenarios = run.Scenarios.Where(x => x.Status == ScenarioStatuses.Pending).ToList().Count;
            int scheduledOrRunningScenarios = run.Scenarios.Where(x => x.IsScheduledOrRunning).ToList().Count;
            if (run.Scenarios.Count == 0)
            {
                throw new Exception("Run has no scenarios");
            }
            else if (completedScenarios == run.Scenarios.Count)
            {
                throw new Exception("Cannot start run because it has already been completed");
            }
            else if (scheduledOrRunningScenarios > 0)
            {
                throw new Exception("Cannot start run because it has already been scheduled or is already running");
            }
            else if (pendingScenarios != run.Scenarios.Count)
            {
                throw new Exception("Cannot start run because all scenarios must have Pending status");
            }

            foreach (var scenario in run.Scenarios)
            {
                if (scenario.Passes.Count == 0)
                {
                    throw new Exception("Scenario has no passes");
                }
            }
        }
    }



}
