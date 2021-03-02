using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks
{
    public interface IBreakDeleted : IEvent
    {
        DateTime DateRangeStart { get; }
        DateTime DateRangeEnd { get; }
        List<string> SalesAreaNames { get; }
    }
}
