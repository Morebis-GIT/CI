using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace xggameplan.Extensions
{
    public static class DateTimeExtensions
    {
        public static Tuple<DateTime, DateTime> StartAndEndOfWeekDate(this DateTime dt,
            DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            var startdate = dt.AddDays(-1 * diff).Date;
            var enddate = startdate.AddDays(6).Date;
            return Tuple.Create(startdate, enddate);
        }

        public static int GetWeekNumber(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date,
                CalendarWeekRule.FirstFourDayWeek, startOfWeek);
        }

        public static List<int> GetDayNumbers(this IEnumerable<string> days)
        {
            var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
            return days.Select(day => (int)daysOfWeek.First(d => d.ToString().StartsWithInvariantCultureIgnoreCase(day.Trim())))
                .Select(currentDay => currentDay == 0 ? 7 : currentDay).OrderBy(i => i).ToList();
        }

        public static List<DayOfWeek> GetDaysOfWeek(this IEnumerable<string> days)
        {
            var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();

            return days.Select(day =>
                    daysOfWeek.First(d => d.ToString().StartsWithInvariantCultureIgnoreCase(day.Trim())))
                .OrderBy(i => i).ToList();
        }

        /// <summary>
        ///  If the string[] contains Mon,Tue,Thu,Fri,Sat,Sun, then there will be 2 Tuple records where
        ///item1 and item2 will be 1 to 2 and 4 to 7
        /// </summary>
        /// <param name="days">days in string format</param>
        /// <returns></returns>
        public static IEnumerable<Tuple<int, int>> GetDayRange(this IEnumerable<string> days)
        {
            var dayNumber = days.GetDayNumbers();

            //0,1,2,7 then (0,2),(7,7)
            var started = false;
            int rangeStart = 0, lastItem = 0;

            foreach (int item in dayNumber)
            {
                if (!started)
                {
                    rangeStart = lastItem = item;
                    started = true;
                }
                else if (item == lastItem + 1)
                {
                    lastItem = item;
                }
                else
                {
                    yield return new Tuple<int, int>(rangeStart, lastItem);
                    rangeStart = lastItem = item;
                }
            }

            if (started)
            {
                yield return new Tuple<int, int>(rangeStart, lastItem);
            }
        }

        public static int GetTimeDiffInMin(this IEnumerable<TimeSpan> date)
        {
            date = date?.Distinct().OrderBy(d => d);
            var diffInMinutes = date?.Skip(1).FirstOrDefault().TotalMinutes - date?.FirstOrDefault().TotalMinutes;
            return (int)(diffInMinutes ?? 0);
        }

        public static string GetSelectedDays(this List<DayOfWeek> days)
        {
            // Make monday as week start
            var dayNumbers = days.Distinct().ToList().Select(d =>
            {
                if (d == 0)
                {
                    return 7;
                }

                return (int)d;
            }).OrderBy(d => d).ToList();

            //if selected Y else N
            string selectedDays = string.Empty;
            for (int i = 1; i <= 7; i++)
            {
                if (dayNumbers.Contains(i))
                {
                    selectedDays += "Y";
                }
                else
                {
                    selectedDays += "N";
                }
            }

            return selectedDays;
        }

        //dayCode -
        // ”1111111” Days of the week that values applies to Monday to Sunday
        // where 1 means applies and 0 means does not - this will always have 7 digits

        //If the dayCode ="1101111" which means Mon,Tue,Thu,Fri,Sat,Sun, then there will be 2 Tuple records where
        ///item1 and item2 will be 1 to 2 and 4 to 7
        public static IEnumerable<Tuple<int, int>> GetDayRangeFromDayCode(this string dayCode)
        {
            if (dayCode.Length != 7)
            {
                throw new InvalidDataException("Day code will always have 7 digits");
            }
            var dayNumber = Array.ConvertAll(dayCode.ToCharArray(), c => (int)Char.GetNumericValue(c));
            var started = false;
            int rangeStart = 0, rangeEnd = 0;

            for (var i = 0; i <= 6; i++)
            {
                switch (dayNumber[i])
                {
                    case 0:
                        if (started)
                        {
                            yield return
                                new Tuple<int, int>(rangeStart + 1, rangeEnd + 1);
                            started = false;
                        }
                        break;

                    case 1:
                        rangeEnd = i;
                        if (!started)
                        {
                            rangeStart = i;
                            started = true;
                        }
                        break;

                    default:
                        throw new InvalidDataException("Day code will always have 0 or 1 applies");
                }
            }
            if (started)
            {
                yield return new Tuple<int, int>(rangeStart + 1, rangeEnd + 1); // +1 because item location always index+1
            }
        }

        public static IEnumerable<DayOfWeek> ParseDayOfWeekDayCode(this string dayCode)
        {
            if (dayCode.Length != 7 || dayCode.Any(ch => ch != '1' && ch != '0'))
            {
                throw new ArgumentException("Invalid day code provided");
            }

            var dayNumbers = Array.ConvertAll(dayCode.ToCharArray(), c => (int)Char.GetNumericValue(c));
            var daysContainer = new List<DayOfWeek>();

            for (int i = 0; i < dayNumbers.Length; i++)
            {
                if (dayNumbers[i] != default(int))
                {
                    if (i + 1 == dayNumbers.Length)
                    {
                        daysContainer.Add(DayOfWeek.Sunday);
                    }
                    else
                    {
                        daysContainer.Add((DayOfWeek)(i + 1));
                    }
                }
            }

            return daysContainer;
        }
    }
}
