using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CampaignTargetModel : ICloneable
    {
        public List<StrikeWeightModel> StrikeWeights { get; set; }

        public object Clone()
        {
            CampaignTargetModel campaignTargetModel = (CampaignTargetModel)this.MemberwiseClone();
            if (this.StrikeWeights != null)
            {
                campaignTargetModel.StrikeWeights = new List<StrikeWeightModel>();
                this.StrikeWeights.ForEach(sw => campaignTargetModel.StrikeWeights.Add((StrikeWeightModel)sw.Clone()));
            }
            return campaignTargetModel;
        }
    }
}
