using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings
{
    public interface ITotalRatingDeleted : IEvent
    {
        DateTime DateTimeFrom { get; }

        DateTime DateTimeTo { get; }

        string SalesArea { get; }
    }
}
