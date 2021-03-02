using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace xggameplan.Model
{
    public abstract class CreateRunScenarioModelBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsLibraried { get; set; }
        public bool IsAutopilot { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public string Progress { get; set; }
        public ScenarioStatuses Status { get; set; }
        public List<CreateCampaignPassPriorityModel> CampaignPassPriorities =
            new List<CreateCampaignPassPriorityModel>();
        public CampaignPriorityRoundsModel CampaignPriorityRounds { get; set; }
    }
}
