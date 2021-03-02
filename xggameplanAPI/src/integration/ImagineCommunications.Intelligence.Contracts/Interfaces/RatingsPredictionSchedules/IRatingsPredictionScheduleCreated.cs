using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules
{
    public interface IRatingsPredictionScheduleCreated : IEvent
    {
        string SalesArea { get; }

        DateTime ScheduleDay { get; }

        IEnumerable<RatingModel> Ratings { get; }
    }
}
