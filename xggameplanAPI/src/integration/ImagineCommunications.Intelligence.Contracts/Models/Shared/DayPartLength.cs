using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class DayPartLength
    {
        public DayPartLength(TimeSpan length, int multipartNumber, decimal desiredPercentageSplit, decimal currentPercentageSplit)
        {
            Length = length;
            MultipartNumber = multipartNumber;
            DesiredPercentageSplit = desiredPercentageSplit;
            CurrentPercentageSplit = currentPercentageSplit;
        }

        public TimeSpan Length { get; }
        public int MultipartNumber { get; }
        public decimal DesiredPercentageSplit { get; }
        public decimal CurrentPercentageSplit { get; }
    }
}
