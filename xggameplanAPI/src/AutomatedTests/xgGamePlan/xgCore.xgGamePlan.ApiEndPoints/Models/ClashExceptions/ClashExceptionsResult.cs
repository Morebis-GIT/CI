using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ClashExceptions
{
    public class GetClashExceptionsResult
    {
        public IEnumerable<ClashException> items { get; set; }
        public int totalCount { get; set; }
    }
}
