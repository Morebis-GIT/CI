using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Campaigns_BySalesAreaCampaignTargetAndStatusAndTargetRatings_RangeSortByTargetRatings
        : AbstractIndexCreationTask<Campaign>
    {
        public static string DefaultIndexName =>
            "Campaigns/BySalesAreaCampaignTargetAndStatusAndTargetRatings_RangeSortByTargetRatings";

        public Campaigns_BySalesAreaCampaignTargetAndStatusAndTargetRatings_RangeSortByTargetRatings()
        {
            Map = campaign =>
                from document in campaign
                select new
                {
                    document.SalesAreaCampaignTarget,
                    document.TargetRatings,
                    document.Status
                };
        }
    }
}
