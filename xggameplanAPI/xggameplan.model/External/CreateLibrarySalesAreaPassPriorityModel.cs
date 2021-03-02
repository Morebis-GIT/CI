using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CreateLibrarySalesAreaPassPriorityModel : LibrarySalesAreaPassPriorityModelBase
    {
    }

    public abstract class LibrarySalesAreaPassPriorityModelBase
    {
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string DaysOfWeek { get; set; }
        public List<SalesAreaPriorityModel> SalesAreaPriorities { get; set; }
    }
}
