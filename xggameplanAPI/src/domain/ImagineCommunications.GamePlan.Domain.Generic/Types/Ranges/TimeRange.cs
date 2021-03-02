using System;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges
{
    public readonly struct TimeRange
    {
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        public TimeRange(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
