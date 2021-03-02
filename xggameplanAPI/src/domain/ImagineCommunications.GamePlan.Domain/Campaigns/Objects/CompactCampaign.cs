using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CompactCampaign
    {
        public Guid Uid { get; set; }
        public int CustomId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public string CampaignGroup { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string ProductExternalRef { get; set; }
        public string ProductName { get; set; }
        public string AdvertiserName { get; set; }
        public string AgencyName { get; set; }
        public string BusinessType { get; set; }
        public CampaignDeliveryType DeliveryType { get; set; }
        public string Demographic { get; set; }
        public double RevenueBudget { get; set; }
        public decimal TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public bool IsPercentage { get; set; }
        public bool IncludeOptimisation { get; set; }
        public bool TargetZeroRatedBreaks { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public IncludeRightSizer IncludeRightSizer { get; set; }
        public int DefaultCampaignPassPriority { get; set; }
        public string ClashCode { get; set; }
        public string ClashDescription { get; set; }
        public AgencyGroupModel AgencyGroup { get; set; }
        public string ReportingCategory { get; set; }
        public string SalesExecutiveName { get; set; }
        public bool StopBooking { get; set; }
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
        public List<CampaignPayback> CampaignPaybacks { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is CompactCampaign)
            {
                var model = obj as CompactCampaign;
                if (model.Uid != Uid)
                {
                    return false;
                }
                if (!int.Equals(model.CustomId, CustomId))
                {
                    return false;
                }
                if (!int.Equals(model.DefaultCampaignPassPriority, DefaultCampaignPassPriority))
                {
                    return false;
                }
                if (!string.Equals(model.Status, Status, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.Name, Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.ExternalId, ExternalId, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.CampaignGroup, CampaignGroup, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.ProductExternalRef, ProductExternalRef, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.ProductName, ProductName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.AdvertiserName, AdvertiserName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.AgencyName, AgencyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.BusinessType, BusinessType, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!Enum.Equals(model.DeliveryType, DeliveryType))
                {
                    return false;
                }
                if (!string.Equals(model.Demographic, Demographic, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.ClashCode, ClashCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.ClashDescription, ClashDescription, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!DateTime.Equals(model.StartDateTime, StartDateTime))
                {
                    return false;
                }
                if (!DateTime.Equals(model.EndDateTime, EndDateTime))
                {
                    return false;
                }
                if (!double.Equals(model.RevenueBudget, RevenueBudget))
                {
                    return false;
                }
                if (!double.Equals(model.TargetRatings, TargetRatings))
                {
                    return false;
                }
                if (!double.Equals(model.ActualRatings, ActualRatings))
                {
                    return false;
                }
                if (!bool.Equals(model.IsPercentage, IsPercentage))
                {
                    return false;
                }
                if (!bool.Equals(model.IncludeOptimisation, IncludeOptimisation))
                {
                    return false;
                }
                if (!bool.Equals(model.TargetZeroRatedBreaks, TargetZeroRatedBreaks))
                {
                    return false;
                }
                if (!bool.Equals(model.InefficientSpotRemoval, InefficientSpotRemoval))
                {
                    return false;
                }
                if (!model.IncludeRightSizer.Equals(IncludeRightSizer))
                {
                    return false;
                }
                if (model.AgencyGroup is null && AgencyGroup != null)
                {
                    return false;
                }
                if (model.AgencyGroup != null && !model.AgencyGroup.Equals(AgencyGroup))
                {
                    return false;
                }
                if (!string.Equals(model.ReportingCategory, ReportingCategory, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!string.Equals(model.SalesExecutiveName, SalesExecutiveName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!bool.Equals(model.StopBooking, StopBooking))
                {
                    return false;
                }
                if (!double.Equals(model.TargetXP, TargetXP))
                {
                    return false;
                }
                if (!double.Equals(model.RevenueBooked, RevenueBooked))
                {
                    return false;
                }
                if (!DateTime.Equals(model.CreationDate, CreationDate))
                {
                    return false;
                }
                if (!bool.Equals(model.AutomatedBooked, AutomatedBooked))
                {
                    return false;
                }
                if (!Enum.Equals(model.TopTail, TopTail))
                {
                    return false;
                }
                if (!int.Equals(model.Spots, Spots))
                {
                    return false;
                }
                if (!string.Equals(model.ActiveLength, ActiveLength, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (!decimal.Equals(model.RatingsDifferenceExcludingPayback, RatingsDifferenceExcludingPayback))
                {
                    return false;
                }
                if (!decimal.Equals(model.ValueDifference, ValueDifference))
                {
                    return false;
                }
                if (!decimal.Equals(model.ValueDifferenceExcludingPayback, ValueDifferenceExcludingPayback))
                {
                    return false;
                }
                if (!decimal.Equals(model.AchievedPercentageTargetRatings, AchievedPercentageTargetRatings))
                {
                    return false;
                }
                if (!decimal.Equals(model.AchievedPercentageRevenueBudget, AchievedPercentageRevenueBudget))
                {
                    return false;
                }
                if (model.CampaignPaybacks is null && CampaignPaybacks != null)
                {
                    return false;
                }
                if (model.CampaignPaybacks != null)
                {
                    if (CampaignPaybacks is null || CampaignPaybacks.Count != model.CampaignPaybacks.Count)
                    {
                        return false;
                    }

                    var newOrderedPaybacks = model.CampaignPaybacks.OrderBy(x => x.GetSortHash());
                    var currentOrderedPaybacks = CampaignPaybacks.OrderBy(x => x.GetSortHash()).ToList();

                    if (newOrderedPaybacks.Where((t, i) => !t.Equals(currentOrderedPaybacks[i])).Any())
                    {
                        return false;
                    }
                }
                return true;
            }
            return base.Equals(obj);
        }
    }
}
