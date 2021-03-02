using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Dto.Internal
{
    internal class RunDto
    {
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
        public bool SpreadProgramming { get; set; }
        public bool SkipLockedBreaks { get; set; }
        public bool IgnorePremiumCategoryBreaks { get; set; }
        public bool ExcludeBankHolidays { get; set; }
        public bool ExcludeSchoolHolidays { get; set; }
        public string Objectives { get; set; }
        public DateTime CreatedOrExecuteDateTime { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public RunStatus RunStatus { get; set; }
        public List<RunScenario> RunScenarios { get; set; } = new List<RunScenario>();
        public List<Scenario> Scenarios { get; set; } = new List<Scenario>();
        public List<Pass> Passes { get; set; } = new List<Pass>();
    }
}
