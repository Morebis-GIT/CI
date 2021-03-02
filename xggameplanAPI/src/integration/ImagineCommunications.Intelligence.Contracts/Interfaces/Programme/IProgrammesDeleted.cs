using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme
{
    public interface IProgrammesDeleted : IEvent
    {
        string SalesArea { get; }

        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }
}
