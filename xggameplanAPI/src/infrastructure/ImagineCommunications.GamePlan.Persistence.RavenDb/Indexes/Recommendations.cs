using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Recommendations_Default : AbstractIndexCreationTask<Recommendation>
    {
        public static string DefaultIndexName => "Recommendations/Default";

        public Recommendations_Default()
        {
            Map = recommendations =>
                from recommendation in recommendations
                select new
                {
                    recommendation.ScenarioId,
                    recommendation.Processor
                };
        }
    }
}
