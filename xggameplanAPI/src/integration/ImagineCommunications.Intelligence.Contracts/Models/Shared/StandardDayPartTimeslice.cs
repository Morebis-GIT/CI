using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class StandardDayPartTimeslice
    {
        public int StartDay { get; }
        public int EndDay { get; }
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        public StandardDayPartTimeslice(int startDay, int endDay, TimeSpan startTime, TimeSpan endTime)
        {
            StartDay = startDay;
            EndDay = endDay;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
