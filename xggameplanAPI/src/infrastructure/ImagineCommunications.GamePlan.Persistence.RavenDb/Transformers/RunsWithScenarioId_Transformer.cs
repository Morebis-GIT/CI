using System.Linq;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class RunsWithScenarioId_Transformer
        : AbstractTransformerCreationTask<Run>
    {
        public RunsWithScenarioId_Transformer()
        {
            TransformResults = runs =>
                from run in runs
                from scenario in run.Scenarios
                select new
                {
                    run.Id,
                    ScenarioId = scenario.Id
                };
        }
    }
}
