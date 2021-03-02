using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Restrictions
{
    public class GetRestrictionsResult
    {
        public IEnumerable<Restriction> items { get; set; }
        public int totalCount { get; set; }
    }
}
