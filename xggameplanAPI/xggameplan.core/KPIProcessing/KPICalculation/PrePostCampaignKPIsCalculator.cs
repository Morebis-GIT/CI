using System;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using xggameplan.Model;

namespace xggameplan.KPIProcessing.KPICalculation
{
    public static class PrePostCampaignKPIsCalculator
    {
        public static void PopulatePrePostCampaignKPIs(ICampaignKpiData campaignData, ScenarioCampaignResultItem source, ScenarioCampaignResultModel result)
        {
            var totalNominalValue = source.NominalValue + campaignData.NominalValue;
            var revenueBudget = campaignData.RevenueBudget ?? 0;
            var totalPayback = campaignData.Payback ?? 0;
            var differenceValueDelivered = totalNominalValue - revenueBudget;
            var differenceValueDeliveredPayback = totalNominalValue - revenueBudget - totalPayback;
            var dayPartRevenueWithPayback = revenueBudget + totalPayback;

            result.ZeroRatedSpots = (int)source.ZeroRatedSpots;
            result.NominalValue = source.NominalValue;
            result.TotalNominalValue = totalNominalValue;
            result.RevenueBudget = revenueBudget;
            result.TotalPayback = totalPayback;
            result.DifferenceValueDelivered = differenceValueDelivered;
            result.DifferenceValueDeliveredPayback = differenceValueDeliveredPayback;
            result.DifferenceValueDeliveredPercentage = revenueBudget == 0
                ? 0
                : Math.Round((differenceValueDelivered / revenueBudget) * 100, 2);
            result.DifferenceValueDeliveredPercentagePayback = dayPartRevenueWithPayback == 0
                ? 0
                : Math.Round((differenceValueDeliveredPayback / dayPartRevenueWithPayback) * 100, 2);
        }
    }
}
