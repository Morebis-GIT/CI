using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace ImagineCommunications.GamePlan.Domain.Campaigns
{
    public static class CampaignGroupingExtensions
    {
        /// <summary>
        /// Concatenates the <see cref="CampaignWithProductFlatModel.ActiveLength"/> of the grouped Campaigns
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groupedItems"></param>
        /// <returns></returns>
        public static string AggregateActiveLength<TKey>(this IGrouping<TKey, CampaignWithProductFlatModel> groupedItems)
        {
            var distinctActiveLength = groupedItems.Select(e => e.ActiveLength)
                .Where(e => !string.IsNullOrEmpty(e))
                .SelectMany(e => e.Split('/'))
                .Select(int.Parse)
                .Distinct()
                .ToList();

            distinctActiveLength.Sort();
            return string.Join("/", distinctActiveLength);
        }

        /// <summary>
        /// Sums up <see cref="CampaignWithProductFlatModel.AchievedPercentageTargetRatings"/> of the grouped Campaigns
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groupedItems"></param>
        /// <returns></returns>
        public static decimal AggregateAchievedPercentageTargetRatings<TKey>(this IGrouping<TKey, CampaignWithProductFlatModel> groupedItems)
        {
            var targetRatingsSum = groupedItems.Sum(x => x.TargetRatings);
            if (targetRatingsSum == 0m)
            {
                return 0m;
            }

            return (groupedItems.Sum(x => x.ActualRatings) / targetRatingsSum) * 100m;
        }

        /// <summary>
        /// Sums up <see cref="CampaignWithProductFlatModel.AchievedPercentageRevenueBudget"/> of the grouped Campaigns
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groupedItems"></param>
        /// <returns></returns>
        public static decimal AggregateAchievedPercentageRevenueBudget<TKey>(this IGrouping<TKey, CampaignWithProductFlatModel> groupedItems)
        {
            var revenueBudgetSum = (decimal)groupedItems.Sum(x => x.RevenueBudget);
            if (revenueBudgetSum == 0m)
            {
                return 0m;
            }

            var revenueBookedSum = (decimal)groupedItems.Sum(x => x.RevenueBooked ?? 0d);
            return (revenueBookedSum / revenueBudgetSum) * 100m;
        }
    }
}
