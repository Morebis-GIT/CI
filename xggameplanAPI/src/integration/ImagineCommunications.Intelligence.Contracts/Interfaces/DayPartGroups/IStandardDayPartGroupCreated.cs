using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups
{
    public interface IStandardDayPartGroupCreated : IEvent
    {
        int GroupId { get; }
        string SalesArea { get; }
        string Demographic { get; }
        bool Optimizer { get; }
        bool Policy { get; }
        bool RatingReplacement { get; }
        List<StandardDayPartSplit> Splits { get; }
    }
}
