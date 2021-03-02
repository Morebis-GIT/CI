using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions
{
    public static class LibrarySalesAreaPassPriorityMappingExtensions
    {
        private static DayOfWeek[] DaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToArray();

        public static string GetStringDowPattern(this SortedSet<DayOfWeek> DowPattern, DayOfWeek startsFrom)
        {
            if(DowPattern == null)
            {
                return null;
            }

            var daysOfWeek = GetAlignedDaysOfWeek(startsFrom);

            return string.Join(string.Empty, daysOfWeek.Select(day => DowPattern.Contains(day) ? "1" : "0"));
        }

        public static SortedSet<DayOfWeek> GetSortedSetDowPattern(this string DowPattern, DayOfWeek startsFrom)
        {
            if(DowPattern == null)
            {
                return null;
            }

            var daysOfWeek = GetAlignedDaysOfWeek(startsFrom);

            return new SortedSet<DayOfWeek>(daysOfWeek.Where((d, idx) => DowPattern[idx] != '0'));
        }

        private static IEnumerable<DayOfWeek> GetAlignedDaysOfWeek(DayOfWeek startDay)
        {
            return DaysOfWeek
                .Skip((int) startDay)
                .Concat(DaysOfWeek.Take((int) startDay))
                .ToArray();
        }
    }
}
