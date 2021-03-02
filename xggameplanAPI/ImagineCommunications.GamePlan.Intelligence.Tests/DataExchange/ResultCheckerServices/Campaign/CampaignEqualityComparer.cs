using System.Collections.Generic;
using DomainCampaign = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.Campaign;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Campaign
{
    internal class CampaignEqualityComparer : IEqualityComparer<DomainCampaign>
    {
        public bool Equals(DomainCampaign x, DomainCampaign y)
        {
            return x != null && y != null &&
                x.CustomId == y.CustomId &&
                x.Name == y.Name &&
                x.ActiveLength == y.ActiveLength &&
                x.RatingsDifferenceExcludingPayback == y.RatingsDifferenceExcludingPayback &&
                x.ValueDifference == y.ValueDifference &&
                x.ValueDifferenceExcludingPayback == y.ValueDifferenceExcludingPayback &&
                x.AchievedPercentageTargetRatings == y.AchievedPercentageTargetRatings &&
                x.AchievedPercentageRevenueBudget == y.AchievedPercentageRevenueBudget;
        }

        public int GetHashCode(DomainCampaign obj)
        {
            int hashCode = -810276762;
            hashCode = hashCode * -1521134295 + obj.CustomId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.ActiveLength);
            hashCode = hashCode * -1521134295 + obj.RatingsDifferenceExcludingPayback.GetHashCode();
            hashCode = hashCode * -1521134295 + obj.ValueDifference.GetHashCode();
            hashCode = hashCode * -1521134295 + obj.ValueDifferenceExcludingPayback.GetHashCode();
            hashCode = hashCode * -1521134295 + obj.AchievedPercentageTargetRatings.GetHashCode();
            hashCode = hashCode * -1521134295 + obj.AchievedPercentageRevenueBudget.GetHashCode();
            return hashCode;
        }
    }
}
