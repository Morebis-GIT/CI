using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class Run : IUniqueIdentifierPrimaryKey
    {
        public const string SearchField = "TokenizedName";

        public Guid Id { get; set; }
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
        public RunInventoryLock InventoryLock { get; set; }
        public bool Real { get; set; }
        public bool Smooth { get; set; }
        public DateTime SmoothDateStart { get; set; }
        public DateTime SmoothDateEnd { get; set; }
        public bool ISR { get; set; }
        public DateTime ISRDateStart { get; set; }
        public DateTime ISRDateEnd { get; set; }
        public bool Optimisation { get; set; }
        public DateTime OptimisationDateStart { get; set; }
        public DateTime OptimisationDateEnd { get; set; }
        public bool RightSizer { get; set; }
        public DateTime RightSizerDateStart { get; set; }
        public DateTime RightSizerDateEnd { get; set; }
        public bool SpreadProgramming { get; set; } = false;
        public bool SkipLockedBreaks { get; set; } = false;
        public bool IgnorePremiumCategoryBreaks { get; set; } = false;
        public bool ExcludeBankHolidays { get; set; } = false;
        public bool ExcludeSchoolHolidays { get; set; } = false;
        public bool? IncludeEfficiencyFactor { get; set; }
        public string Objectives { get; set; }
        public RunAuthor Author { get; set; }
        public EfficiencyCalculationPeriod EfficiencyPeriod { get; set; }
        public int? NumberOfWeeks { get; set; }
        public PositionInProgramme PositionInProgramme { get; set; }
        public bool IgnoreZeroPercentageSplit { get; set; } = false;
        public bool BookTargetArea { get; set; } = false;
        public ICollection<RunSalesAreaPriority> SalesAreaPriorities { get; set; } = new HashSet<RunSalesAreaPriority>();
        public ICollection<RunCampaignReference> Campaigns { get; set; } = new HashSet<RunCampaignReference>();
        public ICollection<RunCampaignProcessesSettings> CampaignsProcessesSettings { get; set; } =
            new HashSet<RunCampaignProcessesSettings>();
        public ICollection<RunScenario> Scenarios { get; set; } = new HashSet<RunScenario>();
        public List<int> FailureTypes { get; set; } = new List<int>();
        public bool Manual { get; set; }
        public RunStatus RunStatus { get; set; }
        public DateTime CreatedOrExecuteDateTime { get; private set; }
        public ICollection<RunInventoryStatus> ExcludedInventoryStatuses { get; set; }
        public int BRSConfigurationTemplateId { get; set; }
        public int RunTypeId { get; set; }
        public ICollection<RunAnalysisGroupTarget> AnalysisGroupTargets { get; set; } = new HashSet<RunAnalysisGroupTarget>();

        public RunScheduleSettings ScheduleSettings { get; set; }
    }
}
