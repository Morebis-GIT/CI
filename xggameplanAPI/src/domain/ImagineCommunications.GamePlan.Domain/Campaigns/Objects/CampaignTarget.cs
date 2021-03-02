using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class Length : ICloneable
    {
        public int MultipartNumber { get; set; }
        public Duration length { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

        public void Validation(Duration lengthValue)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() { FieldName = "Length", FieldToValidate = lengthValue},
                }
            };
            validation.Execute();
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class CampaignTarget : ICloneable
    {
        public List<StrikeWeight> StrikeWeights { get; set; }

        public object Clone()
        {
            CampaignTarget campaignTarget = (CampaignTarget)MemberwiseClone();

            if (StrikeWeights != null)
            {
                campaignTarget.StrikeWeights = new List<StrikeWeight>();
                StrikeWeights.ForEach(sw => campaignTarget.StrikeWeights.Add((StrikeWeight)sw.Clone()));
            }

            return campaignTarget;
        }

        public void Validation(List<StrikeWeight> strikeWeights, DateTime campaignStartDateTime, DateTime campaignEndDateTime)
        {
            if (strikeWeights != null && strikeWeights.Any())
            {
                strikeWeights.ForEach(s => s.RequiredFieldValidation(s.StartDate, s.EndDate, s.Lengths, s.DayParts, s.SpotMaxRatings));
                strikeWeights.ForEach(s => s.CompareValidation(s.StartDate, s.EndDate));
            }
        }
    }

    public class SalesAreaCampaignTarget : ICloneable
    {
        /// <summary>
        /// Property will be removed When channel group changes included
        /// </summary>
        public string SalesArea { get; set; }
        public bool StopBooking { get; set; }
        public SalesAreaGroup SalesAreaGroup { get; set; }
        public List<Multipart> Multiparts { get; set; }
        public List<CampaignTarget> CampaignTargets { get; set; }

        public object Clone()
        {
            SalesAreaCampaignTarget salesAreaCampaignTarget = (SalesAreaCampaignTarget)MemberwiseClone();

            if (SalesAreaGroup != null)
            {
                salesAreaCampaignTarget.SalesAreaGroup = new SalesAreaGroup();
                salesAreaCampaignTarget.SalesAreaGroup = (SalesAreaGroup)SalesAreaGroup.Clone();
            }
            if (Multiparts != null)
            {
                salesAreaCampaignTarget.Multiparts = new List<Multipart>();
                Multiparts.ForEach(mp => salesAreaCampaignTarget.Multiparts.Add((Multipart)mp.Clone()));
            }
            if (CampaignTargets != null)
            {
                salesAreaCampaignTarget.CampaignTargets = new List<CampaignTarget>();
                CampaignTargets.ForEach(ct => salesAreaCampaignTarget.CampaignTargets.Add((CampaignTarget)ct.Clone()));
            }

            return salesAreaCampaignTarget;
        }

        public void Validation(SalesAreaGroup salesAreaGroup, List<Multipart> multiparts,
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
