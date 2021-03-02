using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using xggameplan.model.External.Campaign;

namespace xggameplan.Model
{
    public class SalesAreaCampaignTargetModel : ICloneable
    {
        /// <summary>
        /// Property will be removed when channel group changes included
        /// </summary>
        public string SalesArea { get; set; }
        public SalesAreaGroup SalesAreaGroup { get; set; }
        public List<MultipartModel> Multiparts { get; set; }
        public List<CampaignTargetModel> CampaignTargets { get; set; }

        public object Clone()
        {
            SalesAreaCampaignTargetModel salesAreaCampaignTargetModel = (SalesAreaCampaignTargetModel)this.MemberwiseClone();

            if (this.SalesAreaGroup != null)
            {
                salesAreaCampaignTargetModel.SalesAreaGroup = new SalesAreaGroup();
                salesAreaCampaignTargetModel.SalesAreaGroup = (SalesAreaGroup)this.SalesAreaGroup.Clone();
            }
            if (this.Multiparts != null)
            {
                salesAreaCampaignTargetModel.Multiparts = new List<MultipartModel>();
                this.Multiparts.ForEach(mp => salesAreaCampaignTargetModel.Multiparts.Add((MultipartModel)mp.Clone()));
            }
            if (this.CampaignTargets != null)
            {
                salesAreaCampaignTargetModel.CampaignTargets = new List<CampaignTargetModel>();
                this.CampaignTargets.ForEach(ct => salesAreaCampaignTargetModel.CampaignTargets.Add((CampaignTargetModel)ct.Clone()));
            }

            return salesAreaCampaignTargetModel;
        }
    }
}
