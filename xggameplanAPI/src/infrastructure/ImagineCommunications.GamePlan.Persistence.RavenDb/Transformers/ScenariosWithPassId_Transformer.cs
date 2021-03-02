using System.Linq;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class ScenariosWithPassId_Transformer
        : AbstractTransformerCreationTask<Scenario>
    {
        public ScenariosWithPassId_Transformer()
        {
            TransformResults = scenarios =>
                from scenario in scenarios
                from pass in scenario.Passes
                select new
                {
                    scenario.Id,
                    PassId = pass.Id
                };
        }
    }
}
