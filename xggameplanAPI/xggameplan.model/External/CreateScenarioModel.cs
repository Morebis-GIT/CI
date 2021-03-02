using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CreateScenarioModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int CampaignPerformance { get; set; }

        public int SpotPerformance { get; set; }

        public double NewEfficiency { get; set; }

        public string Progress { get; set; }

        public bool IsLibraried { get; set; }

        public List<PassModel> Passes = new List<PassModel>();

        public CampaignPriorityRoundsModel CampaignPriorityRounds { get; set; }
    }
}
