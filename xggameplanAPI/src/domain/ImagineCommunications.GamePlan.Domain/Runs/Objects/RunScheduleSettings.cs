using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class RunScheduleSettings
    {
        public string QueueName { get; set; }

        public DateTime DateTime { get; set; }

        public int Priority { get; set; }

        public string Comment { get; set; }
    }
}
