using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Raven.Client.Linq.Indexing;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Spots_ByManyFields
        : AbstractIndexCreationTask<Spot>
    {
        public static string DefaultIndexName => "Spots/ByManyFields";

        public Spots_ByManyFields()
        {
            Map = spots =>
                from spot in spots
                select new
                {
                    spot.CustomId,
                    spot.Uid,
                    spot.ExternalCampaignNumber,
                    spot.ExternalSpotRef,
                    spot.ExternalBreakNo,
                    spot.ClientPicked,
                    SalesArea = spot.SalesArea.Boost(10),
                    spot.StartDateTime
                };

            IndexSortOptionsStrings.Add("Uid", SortOptions.String);
        }
    }
}
