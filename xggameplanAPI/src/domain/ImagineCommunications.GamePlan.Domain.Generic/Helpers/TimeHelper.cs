using System;

namespace ImagineCommunications.GamePlan.Domain.Generic.Helpers
{
    public static class TimeHelper
    {
        private static readonly TimeSpan DefaultBroadcastDayStartTime = new TimeSpan(6, 0, 0);
        private static readonly TimeSpan DefaultBroadcastDayEndTime = new TimeSpan(5, 59, 59);
        private static readonly TimeSpan TwentyFourHours = new TimeSpan(24, 0, 0);

        public static TimeSpan ConvertToBroadcast(TimeSpan time)
        {
            bool isTimeAfterMidnight = time.CompareTo(DefaultBroadcastDayStartTime) < 0;
            return isTimeAfterMidnight ? time.Add(TwentyFourHours) : time;
        }

        public static bool TimeRangeOverlaps(
                TimeSpan startTime1,
                TimeSpan endTime1,
                TimeSpan startTime2,
                TimeSpan endTime2)
        {
            startTime1 = ConvertToBroadcast(startTime1);
            startTime2 = ConvertToBroadcast(startTime2);
            endTime1 = ConvertToBroadcast(endTime1);
            endTime2 = ConvertToBroadcast(endTime2);

            if (startTime2 >= startTime1 && startTime2 < endTime1)
            {
                return true;
            }
            if (startTime1 >= startTime2 && startTime1 < endTime2)
            {
                return true;
            }
            if (endTime1 > startTime2 && endTime1 <= endTime2)
            {
                return true;
            }
            if (endTime2 > startTime1 && endTime2 <= endTime1)
            {
                return true;
            }
            if (startTime1 < startTime2 && endTime1 > endTime2)
            {
                return true;
            }
            if (startTime2 < startTime1 && endTime2 > endTime1)
            {
                return true;
            }

            return false;
        }
    }
}
