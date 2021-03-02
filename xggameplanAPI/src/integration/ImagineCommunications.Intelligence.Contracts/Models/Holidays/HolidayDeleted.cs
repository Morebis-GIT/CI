using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Holidays
{
    public class HolidayDeleted : IHolidayDeleted
    {
        public HolidayType? HolidayType { get; }
        public string SalesAreaName { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public HolidayDeleted(HolidayType? holidayType, string salesAreaName, DateTime startDate, DateTime endDate)
        {
            HolidayType = holidayType;
            SalesAreaName = salesAreaName;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
