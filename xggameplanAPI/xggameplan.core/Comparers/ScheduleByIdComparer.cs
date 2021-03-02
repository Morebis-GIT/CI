using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;

namespace xggameplan.core.Comparers
{
    public class ScheduleByIdComparer : IEqualityComparer<Schedule>
    {
        public bool Equals(Schedule x, Schedule y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Id == y.Id;
        }

        public int GetHashCode(Schedule schedule) => schedule.Id.GetHashCode();
    }
}
