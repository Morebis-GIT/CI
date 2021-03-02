using System;

namespace ImagineCommunications.GamePlan.Domain.Breaks.Objects
{
    public class BreakSimple
    {
        public string SalesAreaName { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int CustomId { get; set; }
        public string ExternalBreakRef { get; set; }
    }
}
