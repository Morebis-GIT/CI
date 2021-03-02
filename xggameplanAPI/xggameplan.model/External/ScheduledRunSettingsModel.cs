using System;

namespace xggameplan.model.External
{
    public class ScheduledRunSettingsModel
    {
        public string QueueName { get; set; }
        public Guid ScenarioId { get; set; }
        public DateTime DateTime { get; set; }
        public int Priority { get; set; }
        public string Comment { get; set; }
    }
}
