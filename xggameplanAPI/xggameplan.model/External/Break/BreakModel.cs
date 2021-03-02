using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using NodaTime;

namespace xggameplan.Model
{
    public class BreakModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Unique Id
        /// </summary>
        public int CustomId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? BroadcastDate { get; set; }
        public int? ClockHour { get; set; }
        public string SalesArea { get; set; }
        public string BreakType { get; set; }
        public Duration Duration { get; set; }
        public string Avail { get; set; }
        public bool Optimize { get; set; }
        public string ExternalBreakRef { get; set; }
        public string Description { get; set; }
        public string ExternalProgRef { get; set; }
        public BreakPosition PositionInProg { get; set; }
        public double BreakPrice { get; set; }
        public double FloorRate { get; set; }

        public List<Guid> SpotReferencesList;

        public List<BreakEfficiency> BreakEfficiencyList;
    }
}
