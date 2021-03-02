using System;
using System.Collections.Generic;
using xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Runs
{
    public class Run
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
        public int? NumberOfWeeks { get; set; }
        public bool IsLocked { get; set; }
        public InventoryLockModel InventoryLock { get; set; }
        public string Objectives { get; set; }
        public AuthorModel Author { get; set; }
        public IEnumerable<Scenario> Scenarios { get; set; }
    }
}
