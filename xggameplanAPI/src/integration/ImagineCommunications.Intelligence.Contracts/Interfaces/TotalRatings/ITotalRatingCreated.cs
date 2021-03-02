using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings
{
    public interface ITotalRatingCreated : IEvent
    {
        string SalesArea { get; }
        string Demograph { get; }
        DateTime Date { get; }
        int DaypartGroup { get; }
        int Daypart { get; }
        double TotalRatings { get; }
    }
}
