using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class LibrarySalesAreaPassPriorityModel : BaseModel
    {
        public string Name { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool IsDefault { get; set; }

        /// <summary>
        /// Chosen Days of the week for the sales area pass priority from Monday to Sunday
        /// Format: ”1111111” which represents Monday to Sunday in 7 digits
        /// 1 to apply for the day and 0 to NOT apply for the day
        /// </summary>
        public string DaysOfWeek { get; set; }

        public List<SalesAreaPriorityModel> SalesAreaPriorities { get; set; }
    }
}
