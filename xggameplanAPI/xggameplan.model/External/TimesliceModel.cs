using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class TimesliceModel : ICloneable
    {
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        private List<string> dowPattern = new List<string>();

        public object Clone()
        {
            TimesliceModel timesliceModel = (TimesliceModel)this.MemberwiseClone();

            if (this.DowPattern != null)
            {
                timesliceModel.dowPattern = new List<string>();
                this.dowPattern.ForEach(dp => timesliceModel.dowPattern.Add((string)dp.Clone()));
            }

            return timesliceModel;
        }

        public List<string> DowPattern
        {
            get
            {
                return dowPattern;
            }

            set
            {
                this.dowPattern = value;

                var dowMapping = new Dictionary<string, DayOfWeek>()
                {
                    { "Sun", DayOfWeek.Sunday }, { "Mon", DayOfWeek.Monday }, { "Tue", DayOfWeek.Tuesday }, { "Wed", DayOfWeek.Wednesday },
                    { "Thu", DayOfWeek.Thursday }, { "Fri", DayOfWeek.Friday }, { "Sat", DayOfWeek.Saturday}
                };

                value.ForEach(dow =>
                {
                    if (dowMapping.ContainsKey(dow))
                    {
                        daysOfWeek.Add(dowMapping[dow]);
                    }
                });
            }
        }

        private List<DayOfWeek> daysOfWeek = new List<DayOfWeek>();

        public List<DayOfWeek> DaysOfWeek
        {
            get
            {
                return this.daysOfWeek;
            }
        }

        public bool Contains(DateTime time, DateTime periodStart, DateTime periodEnd)
        {
            int startHour = int.Parse(FromTime.Substring(0, 2));
            int startMinute = int.Parse(FromTime.Substring(3, 2));

            int endHour = int.Parse(ToTime.Substring(0, 2));
            int endMinute = int.Parse(ToTime.Substring(3, 2));

            while (periodStart <= periodEnd)
            {
                if (DaysOfWeek.Contains(periodStart.DayOfWeek))
                {
                    var dayPartStart = new DateTime(periodStart.Year, periodStart.Month, periodStart.Day, startHour, startMinute, 0);
                    var dayPartEnd = new DateTime(periodStart.Year, periodStart.Month, periodStart.Day, endHour, endMinute, 59);

                    var result = time >= dayPartStart && time <= dayPartEnd;

                    if (result)
                    {
                        return true;
                    }
                }
                periodStart = periodStart.AddDays(1);
            }

            return false;
        }
    }
}
