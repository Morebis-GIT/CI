using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class Timeslice
    {
        public Timeslice(string fromTime, string toTime, List<string> dowPattern)
        {
            FromTime = fromTime;
            ToTime = toTime;
            DowPattern = dowPattern;
        }

        public string FromTime { get; }
        public string ToTime { get; }

        public List<string> DowPattern { get; }
    }
}
