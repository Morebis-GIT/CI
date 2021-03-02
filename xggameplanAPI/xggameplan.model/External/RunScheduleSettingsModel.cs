using System;

namespace xggameplan.model.External
{
    public class RunScheduleSettingsModel
    {
        public string QueueName { get; set; }

        public DateTime DateTime { get; set; }

        public int Priority { get; set; }

        public string Comment { get; set; }
    }
}
