using System;
using System.Linq;

namespace xggameplan.common.Utilities
{
    /// <summary>
    /// Utilities to format the 7 character string into something more user friendly.
    /// </summary>
    public static class DaypartDayFormattingUtilities
    {
        /// <summary>
        /// Retrieves an array representing week days in an order based on the provided first day of the week.
        /// </summary>
        /// <param name="startDayOfWeek">A week day from which a week should start.</param>
        /// <returns>Weekdays ordered based on a passed first day.</returns>
        public static string[] GetWeekDaysWithCustomStart(DayOfWeek startDayOfWeek)
        {
            var startDay = startDayOfWeek.ToString();

            string[] weekDays = Enum.GetNames(typeof(DayOfWeek));
            var newWeekDays = new string[7];

            int dayIndex = Array.FindIndex(weekDays, row => row == startDay);

            var first = weekDays.Take(dayIndex).ToArray();
            var second = weekDays.Skip(dayIndex).ToArray();

            second.CopyTo(newWeekDays, 0);
            first.CopyTo(newWeekDays, second.Length);

            return newWeekDays;
        }

        /// <summary>
        /// Format 7 character string for DayPartDays to a more user friendly format.
        /// </summary>
        /// <example>"YNYNYYY" = "S - T - T F S"</example>
        /// <param name="weekDays">Weekday first letters ordered from the specified first day of the week <see cref="GetWeekDaysWithCustomStart(DayOfWeek)"/>/>.</param>
        /// <param name="daypartDays">7 character string representing valid and invalid week days using Y and N.</param>
        /// <param name="startDayOfWeek">Custom value to indicate from what day of the week a week should start.</param>
        /// <returns>User friendly string representing valid and invalid week days.</returns>
        public static string FormatWeekDays(string[] weekDays, string daypartDays, DayOfWeek startDayOfWeek)
        {
            if (weekDays is null)
            {
                throw new ArgumentNullException(nameof(weekDays));
            }

            if (String.IsNullOrEmpty(daypartDays))
            {
                throw new ArgumentException("Cannot be null or empty.", nameof(daypartDays));
            }

            if (weekDays.Length != daypartDays.Length)
            {
                throw new ArgumentException("Length of both inputs must match.");
            }

            var startWeekDayIndex = startDayOfWeek == DayOfWeek.Sunday ? 6 : (int)startDayOfWeek - 1;

            var firstPart = daypartDays.Skip(startWeekDayIndex).ToList();
            var secondPart = daypartDays.Take(startWeekDayIndex).ToList();
            firstPart.AddRange(secondPart);

            var result = new string[7];
            weekDays.CopyTo(result, 0);

            for (int i = 0; i < firstPart.Count; i++)
            {
                if (firstPart[i].Equals('N'))
                {
                    result[i] = "-";
                }
            }

            return String.Join(" ", result.Select(o => o.Substring(0, 1)));
        }
    }
}
