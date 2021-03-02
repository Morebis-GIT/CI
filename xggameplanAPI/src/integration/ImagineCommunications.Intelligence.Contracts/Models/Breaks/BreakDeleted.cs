using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Breaks
{
    public class BreakDeleted : IBreakDeleted
    {
        public DateTime DateRangeStart { get; }

        public DateTime DateRangeEnd { get; }

        public List<string> SalesAreaNames { get; }

        public BreakDeleted(DateTime dateRangeStart, DateTime dateRangeEnd, List<string> salesAreaNames)
        {
            DateRangeStart = dateRangeStart;
            DateRangeEnd = dateRangeEnd;
            SalesAreaNames = salesAreaNames;
        }
    }
}
