using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules
{
    public interface IRatingsPredictionSchedulesDeleted : IEvent
    {
        DateTime DateTimeFrom { get; }

        DateTime DateTimeTo { get; }

        string SalesArea { get; }
    }
}
