using System;
using System.Collections.Generic;
using System.Text;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.DeliveryCappingGroup
{
    public class DeliveryCappingGroupModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Percentage { get; set; }
        public bool ApplyToPrice { get; set; }
    }
}
