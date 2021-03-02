using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models
{
    public class SearchResultModel<TItem>
    {
        public IEnumerable<TItem> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
