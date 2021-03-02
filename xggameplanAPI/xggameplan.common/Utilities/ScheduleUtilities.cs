using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using static xggameplan.common.Helpers.LogAsString;

namespace xggameplan.common
{
    public class ScheduleUtilities
    {
        private readonly IEnumerable<ScheduleIndexed<Break, string>> _indexedSchedules;

        public ScheduleUtilities(IEnumerable<ScheduleIndexed<Break, string>> indexedSchedules) =>
            _indexedSchedules = indexedSchedules;

        /// <summary>
        /// Return an indexed schedule based on the <see cref="Break"/> object's sales area and
        /// scheduled date, or null if a match cannot be found.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public (ScheduleIndexed<Break, string> result, string errorMessage) FindScheduleForBreak(Break item)
        {
            if (item is null)
            {
                return (null, "Break instance was null");
            }

            var (salesArea, scheduleDate) = (item.SalesArea, item.ScheduledDate.Date);

            var result = _indexedSchedules
                .Where(s => s.SalesArea == salesArea && s.Date == scheduleDate)
                .OrderByDescending(s => s.BreaksCount)
                .FirstOrDefault();

            if (result is null)
            {
                return (null, $"Did not find schedule break for sales area {salesArea} at {Log(scheduleDate)}");
            }

            return (result, "OK");
        }
    }
}
