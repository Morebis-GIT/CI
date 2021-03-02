using System;

namespace xggameplan.model.Internal.Landmark
{
    public class ScheduledRunSettingsModel
    {
        public string QueueName { get; set; }
        public Guid ScenarioId { get; set; }
        public DateTime DateTime { get; set; }
        public int Priority { get; set; }
        public string Comment { get; set; }
        public int CreatorId { get; set; }
    }
}
