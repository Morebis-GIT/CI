using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class Run : ICloneable
    {
        public virtual Guid Id { get; set; }

        public int CustomId { get; set; }
        public string Description { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }

        public DateTime EndDate { get; set; }
        public TimeSpan EndTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public DateTime? ExecuteStartedDateTime { get; set; }

        public bool IsLocked { get; set; }

        public InventoryLock InventoryLock { get; set; }

        public bool Real { get; set; }

        public bool Smooth { get; set; }
        public DateRange SmoothDateRange { get; set; }

        public bool ISR { get; set; }
        public DateRange ISRDateRange { get; set; }

        public bool Optimisation { get; set; }
        public DateRange OptimisationDateRange { get; set; }

        public bool RightSizer { get; set; }
        public DateRange RightSizerDateRange { get; set; }

        public bool SpreadProgramming { get; set; } = false;

        public bool SkipLockedBreaks { get; set; } = false;

        public bool IgnorePremiumCategoryBreaks { get; set; } = false;

        public bool ExcludeBankHolidays { get; set; } = false;

        public bool ExcludeSchoolHolidays { get; set; } = false;

        public bool? IncludeEfficiencyFactor { get; set; }

        /// <summary>
        /// Include or ignore sales area(s) with 0% revenue split
        /// </summary>
        public bool IgnoreZeroPercentageSplit { get; set; } = false;

        public bool BookTargetArea { get; set; } = false;

        public string Objectives { get; set; }

        public AuthorModel Author { get; set; }

        public EfficiencyCalculationPeriod EfficiencyPeriod { get; set; }
        public int? NumberOfWeeks { get; set; }

        public PositionInProgramme PositionInProgramme { get; set; }

        public List<SalesAreaPriority> SalesAreaPriorities = new List<SalesAreaPriority>();

        public List<CampaignReference> Campaigns { get; set; } = new List<CampaignReference>();

        public List<CampaignRunProcessesSettings> CampaignsProcessesSettings { get; set; } =
            new List<CampaignRunProcessesSettings>(0);

        //public virtual List<Scenario> Scenarios { get; set; } = new List<Scenario>();

        public virtual List<RunScenario> Scenarios { get; set; } = new List<RunScenario>();
        public List<InventoryStatus> ExcludedInventoryStatuses { get; set; }
        public List<AnalysisGroupTarget> AnalysisGroupTargets { get; set; } = new List<AnalysisGroupTarget>();

        public RunScheduleSettings ScheduleSettings { get; set; }

        public List<int> FailureTypes { get; set; } = new List<int>();

        public bool Manual { get; set; }

        public int BRSConfigurationTemplateId { get; set; }

        public int RunTypeId { get; set; }

        public Run()
        {
            Optimisation = true;    // Default
            RightSizer = false;     // Default
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool IsCompleted
        {
            get { return Scenarios.Count(scenario => scenario.IsCompleted) == Scenarios.Count(); }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public List<RunScenario> ScheduledScenarios
        {
            get { return Scenarios.Where(scenario => scenario.Status == ScenarioStatuses.Scheduled).ToList(); }
        }

        public RunStatus RunStatus
        {
            get
            {
                if (Scenarios.Count(scenario => scenario.Status == ScenarioStatuses.Pending) == Scenarios.Count())
                {
                    return RunStatus.NotStarted;
                }
                else if (Scenarios.Any(scenario => scenario.Status == ScenarioStatuses.CompletedError))
                {
                    return RunStatus.Errors;
                }
                else if (Scenarios.Count(scenario => scenario.Status == ScenarioStatuses.CompletedSuccess) == Scenarios.Count())
                {
                    return RunStatus.Complete;
                }
                return RunStatus.InProgress;
            }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public List<RunScenario> CompletedScenarios
        {
            get { return Scenarios.Where(scenario => scenario.IsCompleted).ToList(); }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool HasNonPendingScenario
        {
            get { return Scenarios != null && Scenarios.Count > 0 ? Scenarios.Any(s => !String.Equals(s.Status, ScenarioStatuses.Pending)) : false; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool HasAllScenarioCompletedSuccessfully
        {
            get { return Scenarios != null && Scenarios.Count > 0 ? Scenarios.All(s => s.Status == ScenarioStatuses.CompletedSuccess) : false; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public DateTime? FirstScenarioStartedDateTime
        {
            get { return Scenarios.Where(s => s.StartedDateTime != null).OrderBy(s => s.StartedDateTime).FirstOrDefault()?.StartedDateTime; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public DateTime? LastScenarioCompletedDateTime
        {
            get { return Scenarios.Where(s => s.IsCompleted && s.CompletedDateTime != null).OrderBy(s => s.CompletedDateTime).LastOrDefault()?.CompletedDateTime; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool IsTemplate => Description.ToLower().StartsWith("template ");

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public bool IsOptimiserNeeded => Optimisation || RightSizer || ISR;

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public ExternalScenarioStatus? ExternalStatus =>
            Scenarios
                ?.FirstOrDefault(c => c.ExternalRunInfo != null && c.ExternalRunInfo.ExternalStatus == ExternalScenarioStatus.Accepted)
                ?.ExternalRunInfo.ExternalStatus ??
                Scenarios?.OrderByDescending(c => c?.ExternalRunInfo?.ExternalStatusModifiedDate)
                    ?.FirstOrDefault(c => c.ExternalRunInfo != null)
                    ?.ExternalRunInfo.ExternalStatus;

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public int TotalSteps
        {
            get
            {
                if (!Smooth || Optimisation || RightSizer || ISR)
                {
                    return Smooth ? 32 : 28;
                }
                return 4;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Validates run for saving. Does not have to be valid for run at this stage.
        /// </summary>
        /// <param name="run"></param>
        public static void ValidateForSave(Run run)
        {
            if (String.IsNullOrEmpty(run.Description))
            {
                throw new Exception("Run description must be set");
            }
            if (run.StartDate == default(DateTime) || run.EndDate == default(DateTime) ||
                run.StartDate > run.EndDate)
            {
                throw new Exception("Invalid run dates");
            }
            if (run.StartDate.Kind != DateTimeKind.Utc)
            {
                throw new Exception("StartDate is not in ISO format. The correct format is YYYY-MM-DDThh:mm:ssz (eg. 2020-02-20T06:30:00z)");
            }
            if (run.EndDate.Kind != DateTimeKind.Utc)
            {
                throw new Exception("EndDate is not in ISO format. The correct format is YYYY-MM-DDThh:mm:ssz (eg. 2020-02-20T06:30:00z)");
            }
            if (run.Scenarios == null || run.Scenarios.Count == 0)
            {
                throw new Exception("Run has no scenarios");
            }

            var scenarioIds = new HashSet<Guid>();
            foreach (RunScenario scenario in run.Scenarios)
            {
                if (scenarioIds.Contains(scenario.Id))
                {
                    throw new Exception("Scenario ID must be unique");
                }

                if (scenario.IsScheduledOrRunning)
                {
                    throw new Exception("Cannot update scenario while it is running or scheduled to run");
                }

                if (scenario.IsCompleted)
                {
                    throw new Exception("Cannot update scenario because it has completed");
                }
                _ = scenarioIds.Add(scenario.Id);
            }

            ValidateProcess(run.ISR, run.ISRDateRange, run, "ISR");
            ValidateProcess(run.Smooth, run.SmoothDateRange, run, "Smooth");
            ValidateProcess(run.Optimisation, run.OptimisationDateRange, run, "Optimisation");
            ValidateProcess(run.RightSizer, run.RightSizerDateRange, run, "Right Sizer");

            if (run.EfficiencyPeriod == EfficiencyCalculationPeriod.NumberOfWeeks)
            {
                if (!run.NumberOfWeeks.HasValue)
                {
                    throw new ArgumentException($"The {nameof(run)} argument's {nameof(run.NumberOfWeeks)} property cannot be null.", nameof(run));
                }

                const int minimumNumberOfWeeks = 1;
                const int maximumNumberOfWeeks = 25;

                if (run.NumberOfWeeks < minimumNumberOfWeeks || run.NumberOfWeeks > maximumNumberOfWeeks)
                {
                    throw new ArgumentException(
                        $"Number of weeks value must be between {minimumNumberOfWeeks} and {maximumNumberOfWeeks}");
                }
            }
        }

        private static void ValidateProcess(bool enabled, DateRange processRange, Run run, string processName)
        {
            if (!enabled)
            {
                return;
            }

            ValidateProcessRange(run, processRange, processName);
        }

        private static void ValidateProcessRange(
            Run run,
            DateRange dateRange,
            string processName)
        {
            if (dateRange is null)
            {
                throw new ArgumentException($"{processName} process range should not be empty");
            }

            dateRange.Start = UpdateToUTCTime(dateRange.Start);
            dateRange.End = UpdateToUTCTime(dateRange.End);

            if (dateRange.Start == default || dateRange.End == default)
            {
                throw new ArgumentException($"{processName} process range should not have empty start and end dates");
            }

            if (dateRange.End < dateRange.Start)
            {
                throw new ArgumentException($"{processName} process range start date should be less than or equal to end date");
            }

            if (!IsDateRangeWithInRunPeriod())
            {
                throw new AggregateException($"{processName} range exceed Run Period");
            }

            //------------------------------------------------------------------
            // Local functions

            bool IsDateRangeWithInRunPeriod()
            {
                DateRange result = CreateDateRange(run.StartDate, run.EndDate);

                return result.Contains(dateRange.Start) && result.Contains(dateRange.End);

                //--------------------------------------------------------------
                // Local functions
                DateRange CreateDateRange(DateTime startDate, DateTime endDate) =>
                    new DateRange(startDate.Date, endDate.Date);
            }
        }

        /// <summary>
        /// Add an hour before converting to UTC time is the date is anhour behind
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime UpdateToUTCTime(DateTime date)
        {
            var utcDate = date.ToUniversalTime();

            if (date.Subtract(utcDate).TotalHours == 1)
            {
                return utcDate.Add(new TimeSpan(1, 0, 0));
            }

            return utcDate;
        }

        /// <summary>
        /// Validates for deleting
        /// </summary>
        /// <param name="run"></param>
        public static void ValidateForDelete(Run run)
        {
            if (run.Scenarios.Find(x => x.IsScheduledOrRunning) != null)
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
        public static void ValidateForDeleteScenarios(Run run)
        {
            if (run.Scenarios.Find(x => x.IsScheduledOrRunning) != null)
            {
                throw new Exception("Cannot delete scenarios that are running or scheduled to run");
            }
        }

        /// <summary>
        /// Validates for deleting scenario
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenario"></param>
        public static void ValidateForDeleteScenario(Run run, RunScenario scenario)
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
        public static void ValidateForRun(Run run)
        {
            int completedScenarios = run.Scenarios.Count(x => x.IsCompleted);
            int pendingScenarios = run.Scenarios.Count(x => x.Status == ScenarioStatuses.Pending);
            int scheduledOrRunningScenarios = run.Scenarios.Count(x => x.IsScheduledOrRunning);

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
        }
    }
}
