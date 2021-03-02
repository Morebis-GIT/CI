using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.RunTypes.Objects
{
    public class RunLandmarkScheduleSettings : ICloneable
    {
        public bool SendToLandmarkAutomatically { get; set; }

        public string QueueName { get; set; }

        public List<DayOfWeek> DaysOfWeek { get; set; }

        public TimeSpan? ScheduledTime { get; set; }

        public int Priority { get; set; }

        public string Comment { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
