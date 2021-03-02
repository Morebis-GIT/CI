using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules
{
    public interface ISpotBookingRuleCreated : IEvent
    {
        List<string> SalesAreas { get; }
        TimeSpan SpotLength { get; }
        TimeSpan MinBreakLength { get; }
        TimeSpan MaxBreakLength { get; }
        int MaxSpots { get; }
        string BreakType { get; }
    }
}
