using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.SharedModels
{
    public class TimeAndDow
    {
        public TimeAndDow(TimeSpan? startTime, TimeSpan? endTime, string daysOfWeek)
        {
            StartTime = startTime;
            EndTime = endTime;
            DaysOfWeek = daysOfWeek;
        }

        public TimeSpan? StartTime { get; }

        public TimeSpan? EndTime { get; }

        public string DaysOfWeek { get; }
    }
}
