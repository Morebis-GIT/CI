using System;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Tenants
{
    public class TenantTimeSettings
    {
        public string PeakStartTime { get; set; }
        public string PeakEndTime { get; set; }

        public string MidnightStartTime { get; set; }
        public string MidnightEndTime { get; set; }

        public DayOfWeek StartDayOfWeek { get; set; }

        public string SystemLogicalDate { get; set; }
    }
}
