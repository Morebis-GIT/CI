using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CreateCampaignPassPriorityModel
    {
        public string CampaignExternalId { get; set; }
        public List<CreatePassPriorityModel> PassPriorities { get; set; }

        public object Clone()
        {
            var campaignPassPriorityModel = (CreateCampaignPassPriorityModel)MemberwiseClone();

            if (PassPriorities != null)
            {
                campaignPassPriorityModel.PassPriorities = new List<CreatePassPriorityModel>();
                PassPriorities.ForEach(pp => campaignPassPriorityModel.PassPriorities.Add((CreatePassPriorityModel)pp.Clone()));
            }

            return campaignPassPriorityModel;
        }
    }
}
