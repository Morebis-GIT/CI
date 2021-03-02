using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic;

namespace xggameplan.core.Extensions
{
    public static class SearchCampaignExtensions
    {
        /// <summary>
        /// Apply Sorting And Paging for the supplied Campaigns list
        /// </summary>
        /// <param name="queryableCampaignList">The IQueryable Campaigns List </param>
        /// <param name="queryModel">The <see cref="CampaignSearchQueryModel"/> containing information for sorting and paging </param>
        /// <returns>An <see cref="IQueryable{CampaignWithProductFlatModel}"/> containing the sorted and paged Campaigns </returns>
        public static IEnumerable<CampaignWithProductFlatModel> ApplySortingAndPaging(
            this IEnumerable<CampaignWithProductFlatModel> queryableCampaignList, CampaignSearchQueryModel queryModel)
        {
            if (queryModel != null && queryableCampaignList != null)
            {
                //apply sorting
                switch (queryModel.Orderby)
                {
                    case CampaignOrder.IncludeRightSizer:
                    {
                        queryableCampaignList = (queryModel.OrderDirection == OrderDirection.Asc)
                            ? queryableCampaignList.OrderBy(a => a.IncludeRightSizer)
                            : queryableCampaignList.OrderByDescending(a => a.IncludeRightSizer);
                        break;
                    }

                    case CampaignOrder.DeliveryType:
                    {
                        queryableCampaignList = queryModel.OrderDirection == OrderDirection.Asc
                            ? queryableCampaignList.OrderBy(c => c.DeliveryType)
                            : queryableCampaignList.OrderByDescending(c => c.DeliveryType);
                        break;
                    }

                    case CampaignOrder.StartDateTime:
                    case CampaignOrder.EndDateTime:
                    {
                        queryableCampaignList =
                            queryableCampaignList.OrderByDateTimeSingleItem(queryModel.Orderby.ToString(),
                                queryModel.OrderDirection);
                        break;
                    }

                    case CampaignOrder.TargetRatings:
                    case CampaignOrder.RevenueBudget:
                    case CampaignOrder.ActualRatings:
                    {
                        queryableCampaignList =
                            queryableCampaignList.OrderByDoubleSingleItem(queryModel.Orderby.ToString(),
                                queryModel.OrderDirection);
                        break;
                    }

                    case CampaignOrder.InefficientSpotRemoval:
                    case CampaignOrder.IncludeOptimisation:
                    case CampaignOrder.TargetZeroRatedBreaks:
                    {
                        queryableCampaignList = queryableCampaignList.OrderBySingleItem(queryModel.Orderby.ToString(),
                            queryModel.OrderDirection);
                        break;
                    }

                    case CampaignOrder.DiffRatings:
                    {
                        queryableCampaignList = (queryModel.OrderDirection == OrderDirection.Asc)
                            ? queryableCampaignList.OrderBy(a => (a.ActualRatings - a.TargetRatings))
                            : queryableCampaignList.OrderByDescending(a => (a.ActualRatings - a.TargetRatings));
                        break;
                    }

                    case CampaignOrder.TargetDelivery:
                    {
                        var orderedCampaignList = queryableCampaignList.OrderBy(e => e.TargetRatings == default(decimal));
                        queryableCampaignList = (queryModel.OrderDirection == OrderDirection.Asc)
                            ? orderedCampaignList.ThenBy(targetRatingsCampaignSorting)
                            : orderedCampaignList.ThenByDescending(targetRatingsCampaignSorting);
                        break;
                    }

                    default:
                    {
                        queryableCampaignList =
                            queryableCampaignList.OrderByAlphaNumericSingleItem(queryModel.Orderby.ToString(),
                                queryModel.OrderDirection);
                        break;
                    }
                }

                //apply paging
                queryableCampaignList = queryableCampaignList.ApplyPaging(queryModel.Skip, queryModel.Top);
            }

            return queryableCampaignList;
        }

        private static Func<CampaignWithProductFlatModel, decimal?> targetRatingsCampaignSorting = (item) =>
            item.TargetRatings != default(decimal) ? (item.ActualRatings / item.TargetRatings) : default(decimal?);
    }
}
