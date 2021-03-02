using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    public class TimeAndDowAPI
    {
        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Days of the week that the <see cref="TimeAndDow"/> applies to, Monday to Sunday.
        /// "Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday","Mon","Tue","Wed","Thu","Fri","Sat","Sun"
        /// </summary>
        public ICollection<string> DaysOfWeek { get; set; } = new List<string>();

        public TimeAndDowAPI()
        {
        }

        public TimeAndDowAPI(string daysOfWeekBinary)
        {
            SetDaysOfWeek(daysOfWeekBinary);
        }

        public void SetDaysOfWeek(string daysOfWeekBinary)
        {
            if (daysOfWeekBinary is null)
            {
                DaysOfWeek = new List<string>();
                return;
            }

            if (daysOfWeekBinary.Length != 7)
            {
                throw new ArgumentException(nameof(daysOfWeekBinary));
            }

            for (int i = 0; i < daysOfWeekBinary.Length; i++)
            {
                if (daysOfWeekBinary[i] == '1')
                {
                    DaysOfWeek.Add(GetDayOfWeek(i));
                }
            }
        }

        public TimeAndDow ConvertToTimeAndDow()
        {
            return new TimeAndDow
            {
                DaysOfWeek = DaysOfWeekBinary,
                EndTime = EndTime,
                StartTime = StartTime
            };
        }

        public string DaysOfWeekBinary
        {
            get
            {
                var result = new[] {'0', '0', '0', '0', '0', '0', '0'};
                foreach (var day in DaysOfWeek)
                {
                    var dayOfWeek = GetDayOfWeek(day);
                    var index = (int) dayOfWeek - 1;
                    if (index < 0)
                    {
                        index = 6;
                    }

                    result[index] = '1';
                }

                return new string(result);
            }
        }

        private DayOfWeek GetDayOfWeek(string day)
        {
            switch (day.ToUpperInvariant())
            {
                case "MON":
                case "MONDAY":
                    return DayOfWeek.Monday;
                case "TUE":
                case "TUESDAY":
                    return DayOfWeek.Tuesday;
                case "WED":
                case "WEDNESDAY":
                    return DayOfWeek.Wednesday;
                case "THU":
                case "THURSDAY":
                    return DayOfWeek.Thursday;
                case "FRI":
                case "FRIDAY":
                    return DayOfWeek.Friday;
                case "SAT":
                case "SATURDAY":
                    return DayOfWeek.Saturday;
                case "SUN":
                case "SUNDAY":
                    return DayOfWeek.Sunday;
                default:
                    throw new ArgumentException($"{day} is invalid day");
            }
        }

        private string GetDayOfWeek(int indexDayOfWeek)
        {
            switch (indexDayOfWeek)
            {
                case 0:
                    return "MON";
                case 1:
                    return "TUE";
                case 2:
                    return "WED";
                case 3:
                    return "THU";
                case 4:
                    return "FRI";
                case 5:
                    return "SAT";
                case 6:
                    return "SUN";
                default:
                    throw new ArgumentOutOfRangeException(nameof(indexDayOfWeek));
            }
        }
    }
}
