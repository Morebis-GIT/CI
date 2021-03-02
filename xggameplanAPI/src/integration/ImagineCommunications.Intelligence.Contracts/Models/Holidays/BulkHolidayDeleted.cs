using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Holidays
{
    public class BulkHolidayDeleted : IBulkHolidayDeleted
    {
        public BulkHolidayDeleted(IEnumerable<HolidayDeleted> data) => Data = data;

        public IEnumerable<IHolidayDeleted> Data { get; }
    }
}
