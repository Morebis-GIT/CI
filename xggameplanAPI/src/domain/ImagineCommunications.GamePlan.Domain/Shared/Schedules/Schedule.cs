using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;

namespace ImagineCommunications.GamePlan.Domain.Shared.Schedules
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string SalesArea { get; set; }

        public List<Programme> Programmes { get; set; }

        public List<Break> Breaks { get; set; }
    }
}
