using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace xggameplan.Model
{
    public enum HolidayType
    {
        PublicHoliday = 0,
        SchoolHoliday = 1
    }

    public class CreateHoliday
    {
        public HolidayType HolidayType { get; set; }
        public List<string> SalesAreaNames { get; set; }
        public List<DateRange> HolidayDateRanges { get; set; }
    }
}
