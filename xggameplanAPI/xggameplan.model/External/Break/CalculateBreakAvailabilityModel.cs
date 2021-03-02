using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CalculateBreakAvailabilityModel
    {
        public DateTime DateRangeStart { get; set; }
        public DateTime DateRangeEnd { get; set; }
        public List<string> SalesAreaNames { get; set; }
    }
}
