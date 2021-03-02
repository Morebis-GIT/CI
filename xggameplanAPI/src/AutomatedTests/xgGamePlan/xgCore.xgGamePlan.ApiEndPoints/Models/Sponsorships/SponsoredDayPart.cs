using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class SponsoredDayPart
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DaysOfWeek { get; set; }
    }
}
