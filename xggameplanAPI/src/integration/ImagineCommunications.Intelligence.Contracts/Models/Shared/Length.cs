using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class Length
    {
        public Length(int multipartNumber, TimeSpan length, decimal desiredPercentageSplit, decimal currentPercentageSplit)
        {
            MultipartNumber = multipartNumber;
            this.length = length;
            DesiredPercentageSplit = desiredPercentageSplit;
            CurrentPercentageSplit = currentPercentageSplit;
        }

        public int MultipartNumber { get; }
        public TimeSpan length { get; }
        public decimal DesiredPercentageSplit { get; }
        public decimal CurrentPercentageSplit { get; }
    }
}
