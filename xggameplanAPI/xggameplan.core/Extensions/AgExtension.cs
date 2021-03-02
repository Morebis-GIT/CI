using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Runs;

namespace xggameplan.core.Extensions
{
    /// <summary>
    /// Conversion functions to AutoGen formats
    /// </summary>
    public class AgConversions
    {
        public const string broadCastDayValue = "995959";

        public static string ToAgBooleanAsString(bool value)
        {
            return value ? "Y" : "N";
        }

        public static string ToAgTimeAsHHMMSS(DateTime time)
        {
            return time.ToString("HHmmss");
        }

        public static string ToAgTimeAsHHMMSS(TimeSpan time)
        {
            return time.ToString("hhmmss");
        }

        public static string ToAgTimeAsTotalHHMMSS(TimeSpan time)
        {
            if (time.TotalHours >= 24)
            {
                return $"{(int)time.TotalHours}{time.ToString("mmss")}";
            }

            var result = ToAgTimeAsHHMMSS(time);

            return result == "000000" ? "0" : result;
        }

        public static string ToAgDateYYYYMMDDAsString(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        public static int ToAgDaysOfWeekAsInt(IEnumerable<DayOfWeek> daysOfWeek)
        {
            //Days of week; 1=Mo; 2=Tu; 4=We; 8=Th; 16=Fr; 32=Sa; 64=Su
            var values = new Dictionary<DayOfWeek, int>()
            {
                { DayOfWeek.Monday, 1 },
                { DayOfWeek.Tuesday, 2 },
                { DayOfWeek.Wednesday, 4 },
                { DayOfWeek.Thursday, 8 },
                { DayOfWeek.Friday, 16 },
                { DayOfWeek.Saturday, 32 },
                { DayOfWeek.Sunday, 64 }
            };
            int value = 0;
            daysOfWeek.Distinct().ToList().ForEach(dayOfWeek => value += values[dayOfWeek]);
            return value;
        }

        public static int ToAgDaysOfWeekAsInt(IEnumerable<DateTime> dates)
        {
            //Days of week; 1=Mo; 2=Tu; 4=We; 8=Th; 16=Fr; 32=Sa; 64=Su
            var values = new Dictionary<DayOfWeek, int>()
            {
                { DayOfWeek.Monday, 1 },
                { DayOfWeek.Tuesday, 2 },
                { DayOfWeek.Wednesday, 4 },
                { DayOfWeek.Thursday, 8 },
                { DayOfWeek.Friday, 16 },
                { DayOfWeek.Saturday, 32 },
                { DayOfWeek.Sunday, 64 }
            };

            int value = 0;
            dates.Distinct().ToList().ForEach(date => value += values[date.DayOfWeek]);
            return value;
        }

        public static string ToAgBreakTypeCodeAsString(string breakType)
        {
            return breakType.Substring(0, 2);
        }

        public static int ToAgBooleanAs1or0(bool value)
        {
            return value ? 1 : 0;
        }

        public static string ToAgIncludeExcludeEither(IncludeOrExcludeOrEither indexOrExcludeOrEither)
        {
            switch (indexOrExcludeOrEither)
            {
                case IncludeOrExcludeOrEither.I:
                    return "Y";

                case IncludeOrExcludeOrEither.E:
                    return "N";

                case IncludeOrExcludeOrEither.X:
                    return "X";

                default:
                    throw new ArgumentOutOfRangeException(nameof(indexOrExcludeOrEither), indexOrExcludeOrEither, null);
            }
        }

        public static int ToAgDaysAsInt(string[] daysOfWeekArray)
        {
            //1 = Mon; 2 = Tue; 4 = Wed; 8 = Thu; 16 = Fri; 32 = Sat; 64 = Sun.So 127
            int sumOfDay = 0;
            foreach (var day in daysOfWeekArray)
            {
                switch (day.ToUpper())
                {
                    case "MONDAY":
                    case "MON":
                        sumOfDay = sumOfDay + 1;
                        break;

                    case "TUESDAY":
                    case "TUE":
                        sumOfDay = sumOfDay + 2;
                        break;

                    case "WEDNESDAY":
                    case "WED":
                        sumOfDay = sumOfDay + 4;
                        break;

                    case "THURSDAY":
                    case "THU":
                        sumOfDay = sumOfDay + 8;
                        break;

                    case "FRIDAY":
                    case "FRI":
                        sumOfDay = sumOfDay + 16;
                        break;

                    case "SATURDAY":
                    case "SAT":
                        sumOfDay = sumOfDay + 32;
                        break;

                    case "SUNDAY":
                    case "SUN":
                        sumOfDay = sumOfDay + 64;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return sumOfDay;
        }

        public static SortedSet<DayOfWeek> ToDaysOfWeek(IEnumerable<string> daysOfWeekStrings)
        {
            var result = new SortedSet<DayOfWeek>();

            if (daysOfWeekStrings == null)
            {
                return result;
            }

            foreach (var sdow in daysOfWeekStrings)
            {
                switch (sdow.ToUpper())
                {
                    case "MONDAY":
                    case "MON":
                        result.Add(DayOfWeek.Monday);
                        break;

                    case "TUESDAY":
                    case "TUE":
                        result.Add(DayOfWeek.Tuesday);
                        break;

                    case "WEDNESDAY":
                    case "WED":
                        result.Add(DayOfWeek.Wednesday);
                        break;

                    case "THURSDAY":
                    case "THU":
                        result.Add(DayOfWeek.Thursday);
                        break;

                    case "FRIDAY":
                    case "FRI":
                        result.Add(DayOfWeek.Friday);
                        break;

                    case "SATURDAY":
                    case "SAT":
                        result.Add(DayOfWeek.Saturday);
                        break;

                    case "SUNDAY":
                    case "SUN":
                        result.Add(DayOfWeek.Sunday);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return result;
        }

        public static int ToAgDaysAsInt(string dayCode)
        {
            //dayCode -
            // ”1111111” Days of the week that the restriction applies to Monday to Sunday
            // where 1 means applies and 0 means does not - this will always have 7 digits
            var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();

            var daylist = new List<char>();
            daylist.AddRange(dayCode.Trim());

            //1 = Mon; 2 = Tue; 4 = Wed; 8 = Thu; 16 = Fri; 32 = Sat; 64 = Sun.So 127
            int sumOfDay = 0;
            foreach (var days in daysOfWeek)
            {
                switch (days)
                {
                    case DayOfWeek.Monday:
                        if (daylist[0] == '1')
                        {
                            sumOfDay = sumOfDay + 1;
                        }

                        break;

                    case DayOfWeek.Tuesday:
                        if (daylist[1] == '1')
                        {
                            sumOfDay = sumOfDay + 2;
                        }

                        break;

                    case DayOfWeek.Wednesday:
                        if (daylist[2] == '1')
                        {
                            sumOfDay = sumOfDay + 4;
                        }

                        break;

                    case DayOfWeek.Thursday:
                        if (daylist[3] == '1')
                        {
                            sumOfDay = sumOfDay + 8;
                        }

                        break;

                    case DayOfWeek.Friday:
                        if (daylist[4] == '1')
                        {
                            sumOfDay = sumOfDay + 16;
                        }

                        break;

                    case DayOfWeek.Saturday:
                        if (daylist[5] == '1')
                        {
                            sumOfDay = sumOfDay + 32;
                        }

                        break;

                    case DayOfWeek.Sunday:
                        if (daylist[6] == '1')
                        {
                            sumOfDay = sumOfDay + 64;
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return sumOfDay;
        }

        public static bool isValidHHMMSSFormat(string timeString)
        {
            return DateTime.TryParseExact(timeString, "HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time);
        }

        public static bool TryParseHHMMSSFormat(string timeString, out TimeSpan timeSpan)
        {
            return TimeSpan.TryParseExact(timeString, "hhmmss", CultureInfo.InvariantCulture, TimeSpanStyles.None, out timeSpan);
        }

        public static TimeSpan ParseHHMMSSFormat(string timeString)
        {
            return TryParseHHMMSSFormat(timeString, out TimeSpan timeSpan) ? timeSpan : default;
        }

        public static TimeSpan ParseTotalHHMMSSFormat(string timeString, bool isTotal = true)
        {
            if (!int.TryParse(timeString.Substring(0, 2), out int hours))
            {
                throw new ArgumentException(nameof(timeString), "Time string is not in correct format.");
            }

            if (hours >= 24)
            {
                var timeWithoutHours = ParseHHMMSSFormat($"00{timeString.Substring(2, 4)}");

                if (!isTotal)
                {
                    if (hours == 99)
                    {
                        hours = 5;
                    }
                    else if (hours > 24)
                    {
                        hours = hours - 24;
                    }
                }

                return timeWithoutHours.Add(TimeSpan.FromHours(hours));
            }
            else
            {
                return ParseHHMMSSFormat(timeString);
            }
        }

        public static bool IsEndOfBroadCastingDay(TimeSpan time)
        {
            return time.Minutes == 0 && time.Hours == 0 && time.Seconds == 0 ? true : false;
        }

        public static int ToAgEfficiencyPeriod(EfficiencyCalculationPeriod efficiencyCalculationPeriod)
        {
            switch (efficiencyCalculationPeriod)
            {
                case EfficiencyCalculationPeriod.RunPeriod:
                    return 1;

                case EfficiencyCalculationPeriod.NumberOfWeeks:
                    return 2;

                default:
                    throw new ArgumentOutOfRangeException(nameof(efficiencyCalculationPeriod));
            }
        }
    }
}
