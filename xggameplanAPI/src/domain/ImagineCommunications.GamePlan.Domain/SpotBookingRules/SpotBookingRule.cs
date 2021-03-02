using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SpotBookingRules
{
    public class SpotBookingRule
    {
        public int Id { get; set; }
        public List<string> SalesAreas { get; set; }
        public TimeSpan SpotLength { get; set; }
        public TimeSpan MinBreakLength { get; set; }
        public TimeSpan MaxBreakLength { get; set; }
        public int MaxSpots { get; set; }
        public string BreakType { get; set; }
    }
}
