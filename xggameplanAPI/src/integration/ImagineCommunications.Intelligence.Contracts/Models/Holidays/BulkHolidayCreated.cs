using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Holidays
{
    public class BulkHolidayCreated : IBulkHolidayCreated
    {
        public BulkHolidayCreated(IEnumerable<HolidayCreated> data) => Data = data;

        public IEnumerable<IHolidayCreated> Data { get; }
    }
}
