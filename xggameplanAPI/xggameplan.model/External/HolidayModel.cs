using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace xggameplan.Model
{
    public class HolidayModel
    {
        public HolidayType HolidayType { get; set; }
        public List<DateRange> HolidayDateRanges { get; set; }
    }
}
