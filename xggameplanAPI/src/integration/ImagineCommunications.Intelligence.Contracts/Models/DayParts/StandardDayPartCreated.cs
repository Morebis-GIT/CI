using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.DayParts
{
    public class StandardDayPartCreated : IStandardDayPartCreated
    {
        public int DayPartId { get; }
        public string SalesArea { get; }
        public string Name { get; }
        public int Order { get; }
        public List<StandardDayPartTimeslice> Timeslices { get; }

        public StandardDayPartCreated(int dayPartId, string salesArea, string name, int order, List<StandardDayPartTimeslice> timeslices)
        {
            DayPartId = dayPartId;
            SalesArea = salesArea;
            Name = name;
            Order = order;
            Timeslices = timeslices;
        }
    }
}
