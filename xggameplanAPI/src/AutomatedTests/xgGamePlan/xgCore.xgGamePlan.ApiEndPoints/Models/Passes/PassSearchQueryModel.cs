using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Passes
{
    public class PassSearchQueryModel
    {
        public string Name { get; set; }
        public bool? IsLibraried { get; set; }
        public IEnumerable<Order<string>> OrderBy { get; set; }
    }
}
