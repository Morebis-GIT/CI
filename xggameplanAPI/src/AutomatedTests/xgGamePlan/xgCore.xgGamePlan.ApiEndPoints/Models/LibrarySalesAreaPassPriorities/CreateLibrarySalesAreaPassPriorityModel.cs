using System.Collections.Generic;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.LibrarySalesAreaPassPriorities
{
    public class CreateLibrarySalesAreaPassPriorityModel
    {
        public string Name { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string DaysOfWeek { get; set; }

        public IEnumerable<SalesAreaPriorityModel> SalesAreaPriorities { get; set; }
    }
}
