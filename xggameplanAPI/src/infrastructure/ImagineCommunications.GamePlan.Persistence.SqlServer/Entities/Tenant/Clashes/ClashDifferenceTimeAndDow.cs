using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class ClashDifferenceTimeAndDow : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int ClashDifferenceId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Days of the week that the <see cref="TimeAndDow"/> applies to, Monday to Sunday.
        /// "Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday","Mon","Tue","Wed","Thu","Fri","Sat","Sun"
        /// </summary>
        public SortedSet<DayOfWeek> DaysOfWeek { get; set; } = new SortedSet<DayOfWeek>();
    }
}
