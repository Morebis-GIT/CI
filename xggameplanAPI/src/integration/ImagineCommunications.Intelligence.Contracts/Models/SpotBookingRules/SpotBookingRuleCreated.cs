using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.SpotBookingRules
{
    public class SpotBookingRuleCreated : ISpotBookingRuleCreated
    {
        public List<string> SalesAreas { get; }
        public TimeSpan SpotLength { get; }
        public TimeSpan MinBreakLength { get; }
        public TimeSpan MaxBreakLength { get; }
        public int MaxSpots { get; }
        public string BreakType { get; }

        public SpotBookingRuleCreated(List<string> salesAreas, TimeSpan spotLength, TimeSpan minBreakLength, TimeSpan maxBreakLength, int maxSpots, string breakType)
        {
            SalesAreas = salesAreas;
            SpotLength = spotLength;
            MinBreakLength = minBreakLength;
            MaxBreakLength = maxBreakLength;
            MaxSpots = maxSpots;
            BreakType = breakType;
        }
    }
}
