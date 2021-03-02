using System;

namespace ImagineCommunications.GamePlan.Domain.DayParts.Objects
{
    public class StandardDayPartTimeslice
    {
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public StandardDayPartTimeslice(int startDay, int endDay, TimeSpan startTime, TimeSpan endTime)
        {
            StartDay = startDay;
            EndDay = endDay;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
