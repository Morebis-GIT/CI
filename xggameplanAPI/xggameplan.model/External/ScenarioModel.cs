using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace xggameplan.Model
{
    public class ScenarioModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsLibraried { get; set; }
        public bool IsAutopilot { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public Guid? ExternalRunId { get; set; }
        public ExternalScenarioStatus? ExternalStatus { get; set; }
        public DateTime? ExternalStatusModifiedDate { get; set; }

        public string Progress { get; set; }
        public ScenarioStatuses Status { get; set; }
        public List<PassModel> Passes = new List<PassModel>();
        public List<CampaignPassPriorityModel> CampaignPassPriorities =
            new List<CampaignPassPriorityModel>();
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime DateUserModified { get; set; }
        public CampaignPriorityRoundsModel CampaignPriorityRounds { get; set; }
    }
}
