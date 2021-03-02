using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Campaigns_BySearch : AbstractIndexCreationTask<Campaign>
    {
        public static string DefaultIndexName => "Campaigns/BySearch";

        public Campaigns_BySearch()
        {
            Map = campaigns => from c in campaigns
                               select new
                               {
                                   c.CampaignGroup,
                                   c.Name,
                                   c.StartDateTime,
                                   c.EndDateTime,
                                   c.Status,
                                   c.ExternalId,
                                   c.BusinessType,
                                   c.DeliveryType,
                                   c.ActualRatings,
                                   c.TargetRatings,
                                   c.RevenueBudget,
                                   c.DemoGraphic,
                                   c.IncludeOptimisation,
                                   c.TargetZeroRatedBreaks,
                                   c.IncludeRightSizer,
                                   c.RightSizerLevel,
                                   c.InefficientSpotRemoval,
                                   c.IsPercentage,
                                   c.Product,
                                   c.Id,
                                   c.CampaignPassPriority,
                                   c.CustomId,
                                   searchfields = new string[] {
                                        c.CampaignGroup,
                                        c.Name,
                                        c.ExternalId,
                                        c.BusinessType,
                                        c.DemoGraphic,
                                        c.Status
                                    }
                               };
            Index(c => c.searchfields, FieldIndexing.Analyzed);
            Index(c => c.StartDateTime, FieldIndexing.Analyzed);
            Index(c => c.EndDateTime, FieldIndexing.Analyzed);
        }
    }
}
