using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts
{
    public interface IStandardDayPartCreated : IEvent
    {
        int DayPartId { get; }
        string SalesArea { get; }
        string Name { get; }
        int Order { get; }
        List<StandardDayPartTimeslice> Timeslices { get; }
    }
}
