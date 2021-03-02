using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunScheduleSettings
    {
        public int Id { get; set; }

        public string QueueName { get; set; }

        public DateTime DateTime { get; set; }

        public int Priority { get; set; }

        public string Comment { get; set; }

        public Guid RunId { get; set; }

        public Run Run { get; set; }
    }
}
