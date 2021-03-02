using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;

namespace xggameplan.model.External.Campaign
{
    public class SalesAreaCampaignTargetViewModel
    {
        public string SalesArea { get; set; }
        public SalesAreaGroup SalesAreaGroup { get; set; }
        public List<MultipartModel> Multiparts { get; set; }
        public List<CampaignTarget> CampaignTargets { get; set; }

        public object Clone()
        {
            SalesAreaCampaignTargetViewModel salesAreaCampaignTarget = (SalesAreaCampaignTargetViewModel)MemberwiseClone();

            if (SalesAreaGroup != null)
            {
                salesAreaCampaignTarget.SalesAreaGroup = new SalesAreaGroup();
                salesAreaCampaignTarget.SalesAreaGroup = (SalesAreaGroup)SalesAreaGroup.Clone();
            }
            if (Multiparts != null)
            {
                salesAreaCampaignTarget.Multiparts = new List<MultipartModel>();
                Multiparts.ForEach(mp => salesAreaCampaignTarget.Multiparts.Add((MultipartModel)mp.Clone()));
            }
            if (CampaignTargets != null)
            {
                salesAreaCampaignTarget.CampaignTargets = new List<CampaignTarget>();
                CampaignTargets.ForEach(ct => salesAreaCampaignTarget.CampaignTargets.Add((CampaignTarget)ct.Clone()));
            }

            return salesAreaCampaignTarget;
        }

        public void Validation(SalesAreaGroup salesAreaGroup, List<MultipartModel> multiparts,
            List<CampaignTarget> campaignTargets, DateTime campaignStartDateTime, DateTime campaignEndDateTime)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() {FieldName = "SalesArea Group", FieldToValidate = salesAreaGroup},
                    new ValidationInfo() {FieldName = "Multiparts", FieldToValidate = multiparts},
                    new ValidationInfo() {FieldName = "Campaign Targets", FieldToValidate = campaignTargets}
                }
            };
            validation.Execute();
            salesAreaGroup.Validation(salesAreaGroup.GroupName, salesAreaGroup.SalesAreas);
            if (multiparts != null && multiparts.Any())
            {
                multiparts.ForEach(l => l.Validation(l.Lengths));
            }

            if (campaignTargets != null && campaignTargets.Any())
            {
                campaignTargets.ForEach(c => c.Validation(c.StrikeWeights, campaignStartDateTime, campaignEndDateTime));
            }
        }
    }
}
