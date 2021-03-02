using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace xggameplan.tests.ControllerTests
{
    public class DeleteByRangeTestData
    {
        public DateTimeRange ScheduledDatesRange { get; set; }
        public List<string> SalesAreaNames { get; set; }
        public string ParamName { get; set; }
    }
}
