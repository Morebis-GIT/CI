using System;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Generic.Helpers
{
    public static class DayOfWeekHelper
    {
        public static bool DateFallsOnDayOfWeek(string[] daysOfWeek, DateTime date)
        {
            string shortDayOfWeek = date.ToString("ddd");
            string dayOfWeek = date.DayOfWeek.ToString();

            return daysOfWeek.Contains(
                shortDayOfWeek,
                StringComparer.InvariantCultureIgnoreCase)
                ||
                daysOfWeek.Contains(
                     dayOfWeek,
                     StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
