using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe
{
    public interface IUniverseCreated : IEvent
    {
        string SalesArea { get; }

        string Demographic { get; }

        DateTime StartDate { get; }

        DateTime EndDate { get; }

        int UniverseValue { get; }
    }
}
