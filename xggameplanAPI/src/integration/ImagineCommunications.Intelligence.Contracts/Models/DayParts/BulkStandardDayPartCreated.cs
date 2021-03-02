using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.DayParts
{
    public class BulkStandardDayPartCreated : IBulkStandardDayPartCreated
    {
        public IEnumerable<IStandardDayPartCreated> Data { get; }

        public BulkStandardDayPartCreated(IEnumerable<StandardDayPartCreated> data) => Data = data;
    }
}
