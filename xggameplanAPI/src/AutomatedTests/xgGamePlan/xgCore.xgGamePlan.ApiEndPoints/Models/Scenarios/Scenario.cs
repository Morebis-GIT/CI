using System;
using System.Collections.Generic;
using xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios
{
    public class Scenario
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsLibraried { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public string Progress { get; set; }
        public ScenarioStatus Status { get; set; }
        public IEnumerable<Pass> Passes { get; set; }
        public IEnumerable<CampaignPassPriorityModel> CampaignPassPriorities { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
