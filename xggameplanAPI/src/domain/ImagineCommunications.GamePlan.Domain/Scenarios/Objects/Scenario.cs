using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace ImagineCommunications.GamePlan.Domain.Scenarios.Objects
{
    public class Scenario : ICloneable
    {
        private bool _isAutopilot;
        private bool? _isLibraried = false;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CustomId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime DateUserModified { get; set; }
        public CampaignPriorityRounds CampaignPriorityRounds { get; set; }

        /// <summary>
        /// Gets autopilot scenario flag
        /// </summary>
        /// <remarks>Libraried scenario can not be autopilot</remarks>
        public bool IsAutopilot
        {
            get => _isAutopilot;
            set => _isAutopilot = IsLibraried != true && value;
        }

        public bool? IsLibraried // TODO: Can be set to bool after DB update
        {
            get => _isLibraried;
            set
            {
                _isLibraried = value;
                if (_isLibraried == true)
                {
                    IsAutopilot = false;
                }
            }
        }

        public List<PassReference> Passes = new List<PassReference>();
        public List<CampaignPassPriority> CampaignPassPriorities = new List<CampaignPassPriority>();

        public object Clone() => MemberwiseClone();

        public static void ValidateForSave(Scenario scenario)
        {
            if (string.IsNullOrEmpty(scenario.Name))
            {
                throw new Exception("Scenario name must be set");
            }
            if (scenario.Passes == null || scenario.Passes.Count == 0)
            {
                throw new Exception("Scenario has no passes");
            }
            if (!scenario.Passes.Where(p => p.Id == 0).Any() && scenario.Passes.Select(p => p.Id).Distinct().ToList().Count != scenario.Passes.Count)
            {
                throw new Exception("Scenario has duplicate passes");
            }
        }
    }
}
