using System;
using System.Collections.Generic;
using System.ComponentModel;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace xggameplan.Model
{
    public class CampaignModel : ICloneable
    {
        public Guid Uid { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string DemoGraphic { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Product { get; set; }
        public double RevenueBudget { get; set; }
        public decimal TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public string CampaignGroup { get; set; }

        /// <summary>
        /// Is the ratings a value or a percentagw
        /// </summary>
        public bool IsPercentage { get; set; }
        public string Status { get; set; }
        public string BusinessType { get; set; }
        public CampaignDeliveryType DeliveryType { get; set; }
        public bool IncludeOptimisation { get; set; }
        public bool TargetZeroRatedBreaks { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public bool IncludeRightSizer { get; set; }
        public RightSizerLevel? RightSizerLevel { get; set; }
        public string ExpectedClearanceCode { get; set; }
        public int CampaignPassPriority { get; set; }
        public int CampaignSpotMaxRatings { get; set; }
        public List<string> BreakType { get; set; }
        public double? TargetXP { get; set; }
        public double? RevenueBooked { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? AutomatedBooked { get; set; }
        public TopTail? TopTail { get; set; }
        public int? Spots { get; set; }
        public string ActiveLength { get; set; }
        public decimal? RatingsDifferenceExcludingPayback { get; set; }
        public decimal? ValueDifference { get; set; }
        public decimal? ValueDifferenceExcludingPayback { get; set; }
        public decimal? AchievedPercentageTargetRatings { get; set; }
        public decimal? AchievedPercentageRevenueBudget { get; set; }

        public List<SalesAreaCampaignTargetModel> SalesAreaCampaignTarget { get; set; }

        [DefaultValue("")]
        public List<TimeRestriction> TimeRestrictions { get; set; }

        [DefaultValue("")]
        public List<ProgrammeRestrictionViewModel> ProgrammeRestrictions { get; set; }

        public List<CampaignProgrammeModel> ProgrammesList { get; set; }
        public List<CampaignPaybackModel> CampaignPaybacks { get; set; }

        public object Clone()
        {
            CampaignModel campaignModel = (CampaignModel)this.MemberwiseClone();

            if (this.BreakType != null)
            {
                campaignModel.BreakType = new List<string>();
                this.BreakType.ForEach(bt => campaignModel.BreakType.Add((string)bt.Clone()));
            }
            if (this.TimeRestrictions != null)
            {
                campaignModel.TimeRestrictions = new List<TimeRestriction>();
                this.TimeRestrictions.ForEach(tr => campaignModel.TimeRestrictions.Add((TimeRestriction)tr.Clone()));
            }
            if (this.ProgrammeRestrictions != null)
            {
                campaignModel.ProgrammeRestrictions = new List<ProgrammeRestrictionViewModel>();
                this.ProgrammeRestrictions.ForEach(pr => campaignModel.ProgrammeRestrictions.Add((ProgrammeRestrictionViewModel)pr.Clone()));
            }
            if (this.ProgrammesList != null)
            {
                campaignModel.ProgrammesList = new List<CampaignProgrammeModel>();
                this.ProgrammesList.ForEach(pr => campaignModel.ProgrammesList.Add((CampaignProgrammeModel)pr.Clone()));
            }

            if (this.SalesAreaCampaignTarget != null)
            {
                campaignModel.SalesAreaCampaignTarget = new List<SalesAreaCampaignTargetModel>();
                this.SalesAreaCampaignTarget.ForEach(sa => campaignModel.SalesAreaCampaignTarget.Add((SalesAreaCampaignTargetModel)sa.Clone()));
            }

            if (this.CampaignPaybacks != null)
            {
                campaignModel.CampaignPaybacks = new List<CampaignPaybackModel>();
                this.CampaignPaybacks.ForEach(cp => campaignModel.CampaignPaybacks.Add((CampaignPaybackModel)cp.Clone()));
            }

            return campaignModel;
        }
    }
}
