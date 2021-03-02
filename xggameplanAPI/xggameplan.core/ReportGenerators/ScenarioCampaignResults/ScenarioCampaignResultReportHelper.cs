using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;

namespace xggameplan.core.ReportGenerators.ScenarioCampaignResults
{
    /// <summary>
    /// Helper class for <see cref="ScenarioCampaignResultReportCreator"/>.
    /// </summary>
    public static class ScenarioCampaignResultReportHelper
    {
        /// <summary>
        /// Extends <see cref="ScenarioCampaignResultItem"/> with <see cref="CampaignPassPriority"/> properties.
        /// </summary>
        /// <param name="items">Scenario Result Items from Optimizer.</param>
        /// <param name="campaignPassPriorities">Campaign level snapshot.</param>
        /// <param name="mapper">Mapper configuration.</param>
        /// <returns></returns>
        public static IReadOnlyCollection<ScenarioCampaignExtendedResultItem> MapToExtendedResults(
            List<ScenarioCampaignResultItem> items,
            List<CampaignPassPriority> campaignPassPriorities,
            IMapper mapper)
        {
            var compactCampaign = campaignPassPriorities
                .ToDictionary(e => e.Campaign.ExternalId, e => e.Campaign);

            var extendedItems = mapper.Map<List<ScenarioCampaignExtendedResultItem>>(items);

            extendedItems.ForEach(item =>
            {
                var currentCampaign = compactCampaign[item.CampaignExternalId];

                item.MediaSalesGroup = currentCampaign.AgencyGroup?.ShortName;
                item.ProductAssignee = currentCampaign.SalesExecutiveName;
                item.StopBooking = currentCampaign.StopBooking;
                item.CreationDate = currentCampaign.CreationDate;
                item.AutomatedBooked = currentCampaign.AutomatedBooked;
                item.TopTail = currentCampaign.TopTail;
                item.ReportingCategory = currentCampaign.ReportingCategory;
                item.ClashCode = currentCampaign.ClashCode;
                item.AgencyName = currentCampaign.AgencyName;

                item.PassThatDelivered100Percent = item.PassThatDelivered100Percent < 0 ? 0 : item.PassThatDelivered100Percent;
            });

            return extendedItems;
        }

        /// <summary>
        /// Extends <see cref="ScenarioCampaignLevelResultItem"/> with <see cref="CampaignPassPriority"/> properties.
        /// </summary>
        /// <param name="items">Scenario Result Items from Optimizer.</param>
        /// <param name="campaignPassPriorities">Campaign level snapshot.</param>
        /// <param name="mapper">Mapper configuration.</param>
        /// <returns></returns>
        public static IReadOnlyCollection<ScenarioCampaignExtendedResultItem> MapToExtendedResults(
            List<ScenarioCampaignLevelResultItem> items,
            List<CampaignPassPriority> campaignPassPriorities,
            IMapper mapper)
        {
            var compactCampaign = campaignPassPriorities
                .ToDictionary(e => e.Campaign.ExternalId, e => e.Campaign);

            var extendedItems = mapper.Map<List<ScenarioCampaignExtendedResultItem>>(items);

            extendedItems.ForEach(item =>
            {
                var currentCampaign = compactCampaign[item.CampaignExternalId];

                item.MediaSalesGroup = currentCampaign.AgencyGroup?.ShortName;
                item.ProductAssignee = currentCampaign.SalesExecutiveName;
                item.StopBooking = currentCampaign.StopBooking;
                item.CreationDate = currentCampaign.CreationDate;
                item.AutomatedBooked = currentCampaign.AutomatedBooked;
                item.TopTail = currentCampaign.TopTail;
                item.ReportingCategory = currentCampaign.ReportingCategory;
                item.ClashCode = currentCampaign.ClashCode;
                item.AgencyName = currentCampaign.AgencyName;

                item.PassThatDelivered100Percent = item.PassThatDelivered100Percent < 0 ? 0 : item.PassThatDelivered100Percent;
            });

            return extendedItems;
        }
    }
}
