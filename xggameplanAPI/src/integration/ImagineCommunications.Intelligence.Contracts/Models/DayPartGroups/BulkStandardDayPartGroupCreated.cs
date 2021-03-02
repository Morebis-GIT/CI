using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.DayPartGroups
{
    public class BulkStandardDayPartGroupCreated : IBulkStandardDayPartGroupCreated
    {
        public IEnumerable<IStandardDayPartGroupCreated> Data { get; }

        public BulkStandardDayPartGroupCreated(IEnumerable<StandardDayPartGroupCreated> data) => Data = data;
    }
}
