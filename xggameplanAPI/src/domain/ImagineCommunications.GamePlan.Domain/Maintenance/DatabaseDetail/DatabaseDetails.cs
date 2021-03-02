using System;

namespace ImagineCommunications.GamePlan.Domain.Maintenance.DatabaseDetail
{
    public class DatabaseDetails
    {
        public static int DefaultId = 1;

        public int Id { get; set; }

        public string Version { get; set; }

        public DateTime TimeUpdatesApplied { get; set; }
    }
}
