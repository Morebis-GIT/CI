using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Breaks
{
    public class Break
    {
        public DateTime ScheduledDate { get; set; }
        public string SalesArea { get; set; }
        public string BreakType { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Optimize { get; set; }
        public string ExternalBreakRef { get; set; }
        public string Description { get; set; }
        public string ExternalProgRef { get; set; }
    }
}
