using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Spots
{
    public class SearchSpotsQuery
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string SalesArea { get; set; }
    }
}
