using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.model.External;

namespace xggameplan.Model
{
    public class RunModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }

        public DateTime EndDate { get; set; }
        public TimeSpan EndTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public DateTime? ExecuteStartedDateTime { get; set; }

        public bool? IncludeEfficiencyFactor { get; set; }

        public DateTime? FirstScenarioStartedDateTime { get; set; }

        public DateTime? LastScenarioCompletedDateTime { get; set; }

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

        public bool IgnoreZeroPercentageSplit { get; set; }

        public bool BookTargetArea { get; set; }

        public bool IsLocked { get; set; }

        public InventoryLockModel InventoryLock { get; set; }

        public bool HasNonPendingScenario { get; set; }

        public bool HasAllScenarioCompletedSuccessfully { get; set; }

        public string Objectives { get; set; }

        public AuthorModel Author { get; set; }

        public EfficiencyCalculationPeriod? EfficiencyPeriod { get; set; }
        public int? NumberOfWeeks { get; set; }

        public string PositionInProgramme { get; set; }

        public List<SalesAreaPriorityModel> SalesAreaPriorities = new List<SalesAreaPriorityModel>();

        public List<CampaignReferenceModel> Campaigns = new List<CampaignReferenceModel>();

        public List<CampaignRunProcessesSettingsModel> CampaignsProcessesSettings = new List<CampaignRunProcessesSettingsModel>();

        public List<ScenarioModel> Scenarios = new List<ScenarioModel>();

        public List<InventoryStatusModel> ExcludedInventoryStatuses = new List<InventoryStatusModel>();

        public List<AnalysisGroupTargetModel> AnalysisGroupTargets = new List<AnalysisGroupTargetModel>();

        public IEnumerable<RunFaultTypeModel> FaultTypes { get; set; }

        public ExternalScenarioStatus? ExternalStatus { get; set; }

        public bool Manual { get; set; }

        public RunStatus Status { get; set; }

        public int BRSConfigurationTemplateId { get; set; }

        public int RunTypeId { get; set; }

        public RunScheduleSettingsModel ScheduleSettings { get; set; }
    }
}
