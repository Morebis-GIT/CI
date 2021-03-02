using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ClashExceptions
{
    public class ClashExceptionUpdateModel
    {
        public DateTime EndDate { get; set; }
        public IncludeOrExclude IncludeOrExclude { get; set; }
        public IEnumerable<TimeAndDow> TimeAndDows { get; set; }
    }
}
