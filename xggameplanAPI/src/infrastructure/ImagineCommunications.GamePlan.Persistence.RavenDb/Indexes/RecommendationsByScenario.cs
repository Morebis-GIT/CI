using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public sealed class RecommendationsByScenario
        : AbstractIndexCreationTask<Recommendation, RecommendationsByScenarioReduceResult>
    {
        public RecommendationsByScenario()
        {
            Map = recommendations => from r in recommendations
                                     select new
                                     {
                                         r.ScenarioId,
                                         r.ExternalCampaignNumber,
                                         r.Action,
                                         r.SpotRating,
                                         Count = 1
                                     };

            Reduce = results => from result in results
                                group result by new
                                {
                                    result.ScenarioId,
                                    result.ExternalCampaignNumber,
                                    result.Action
                                }
                                into agg
                                select new
                                {
                                    agg.Key.ScenarioId,
                                    agg.Key.ExternalCampaignNumber,
                                    agg.Key.Action,
                                    SpotRating = agg.Sum(x => x.SpotRating),
                                    Count = agg.Sum(x => x.Count)


                                };

            Sort(x => x.Count, SortOptions.Int);
            Sort(x => x.SpotRating, SortOptions.Double);
        }
    }
}
