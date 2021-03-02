using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Holidays
{
    public class HolidayCreated : IHolidayCreated
    {
        public HolidayType HolidayType { get; }
        public List<string> SalesAreaNames { get; }
        public List<DateRange> HolidayDateRanges { get; }

        public HolidayCreated(HolidayType holidayType, List<string> salesAreaNames, List<DateRange> holidayDateRanges)
        {
            HolidayType = holidayType;
            SalesAreaNames = salesAreaNames;
            HolidayDateRanges = holidayDateRanges;
        }
    }
}
