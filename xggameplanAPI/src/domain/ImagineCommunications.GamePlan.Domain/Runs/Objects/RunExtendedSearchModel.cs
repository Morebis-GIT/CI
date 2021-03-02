using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class RunExtendedSearchModel
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
        public bool SpreadProgramming { get; set; }
        public bool SkipLockedBreaks { get; set; }
        public bool IgnorePremiumCategoryBreaks { get; set; }
        public bool ExcludeBankHolidays { get; set; }
        public bool ExcludeSchoolHolidays { get; set; }
        public string Objectives { get; set; }
        public AuthorModel Author { get; set; }
        public List<SalesAreaPriority> SalesAreaPriorities { get; set; } = new List<SalesAreaPriority>();
        public List<CampaignReference> Campaigns { get; set; } = new List<CampaignReference>();
        public List<CampaignRunProcessesSettings> CampaignsProcessesSettings { get; set; }
        public virtual List<RunScenario> Scenarios { get; set; } = new List<RunScenario>();
        public IEnumerable<string> ScenarioIds { get; set; }
        public IEnumerable<string> ScenarioNames { get; set; }
        public IEnumerable<string> PassIds { get; set; }
        public IEnumerable<string> PassNames { get; set; }
    }
}
