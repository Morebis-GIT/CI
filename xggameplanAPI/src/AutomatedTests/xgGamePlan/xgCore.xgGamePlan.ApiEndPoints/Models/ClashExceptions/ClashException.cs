using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ClashExceptions
{
    public class ClashException
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ClashExceptionType FromType { get; set; }
        public ClashExceptionType ToType { get; set; }
        public IncludeOrExclude IncludeOrExclude { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public IEnumerable<TimeAndDow> TimeAndDows { get; set; }
    }
}
