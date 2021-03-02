using System;
using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class TimeRestriction
    {
        public TimeRestriction(List<string> salesAreas, List<string> dowPattern, DateTime startDateTime, DateTime endDateTime, string isIncludeOrExclude)
        {
            SalesAreas = salesAreas;
            DowPattern = dowPattern;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            IsIncludeOrExclude = isIncludeOrExclude?.ToUpperInvariant();
        }

        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
        public string IsIncludeOrExclude { get; }

        public List<string> SalesAreas { get; }

        public List<string> DowPattern { get; }
    }
}
