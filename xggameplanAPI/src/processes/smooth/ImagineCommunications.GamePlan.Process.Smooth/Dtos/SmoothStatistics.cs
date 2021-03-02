using System;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public class SmoothStatistics
    {
        public SmoothStatistics(string salesAreaName) =>
            SalesAreaName = salesAreaName;

        public string SalesAreaName { get; }

        public DateTime TimeStarted { get; set; }

        public DateTime TimeEnded { get; set; }
    }
}
