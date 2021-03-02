using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays
{
    public interface IHolidayCreated : IEvent
    {
        HolidayType HolidayType { get; }
        List<string> SalesAreaNames { get; }
        List<DateRange> HolidayDateRanges { get; }
    }
}
