using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    /// <summary>
    /// Notification object fields need to be camelCase for now.
    /// </summary>
    public class RunNotification
    {
        public Guid id { get; set; }

        public string description { get; set; }

        public RunStatus status { get; set; }

        public DateTime endDate { get; set; }

        public TimeSpan endTime { get; set; }
    }
}
