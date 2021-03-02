using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe
{
    public interface IUniverseDeleted : IEvent
    {
        DateTime? StartDate { get; }

        DateTime? EndDate { get; }

        string SalesArea { get; }

        string Demographic { get; }
    }
}
