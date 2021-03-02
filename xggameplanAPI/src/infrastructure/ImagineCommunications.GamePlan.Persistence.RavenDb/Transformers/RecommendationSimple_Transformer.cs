using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class RecommendationSimple_Transformer
        : AbstractTransformerCreationTask<Recommendation>
    {
        public RecommendationSimple_Transformer()
        {
            TransformResults = recommendations =>
                from recommendation in recommendations
                select new
                {
                    recommendation.ScenarioId,
                    recommendation.ExternalSpotRef,
                    recommendation.SalesArea,
                    recommendation.Demographic,
                    recommendation.Processor,
                    recommendation.SpotEfficiency,
                    recommendation.ExternalCampaignNumber,
                    recommendation.OptimiserPassSequenceNumber,
                    recommendation.Product,
                    recommendation.StartDateTime,
                    recommendation.EndDateTime,
                    recommendation.SpotLength,
                    recommendation.SpotRating
                };
        }
    }
}
