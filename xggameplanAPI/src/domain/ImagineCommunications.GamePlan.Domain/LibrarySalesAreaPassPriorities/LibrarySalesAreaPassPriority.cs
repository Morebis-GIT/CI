using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;

namespace ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities
{
    public class LibrarySalesAreaPassPriority : EntityBase
    {
        public string Name { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Chosen Days of the week for the sales area pass priority from Monday to Sunday
        /// Format: ”1111111” which represents Monday to Sunday in 7 digits
        /// 1 to apply for the day and 0 to NOT apply for the day
        /// </summary>
        public string DaysOfWeek { get; set; }

        public List<SalesAreaPriority> SalesAreaPriorities { get; set; }
    }
}
