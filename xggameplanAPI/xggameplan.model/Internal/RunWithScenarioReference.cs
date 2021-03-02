using System;

namespace xggameplan.Model
{
    public class RunWithScenarioReference
    {
        public RunWithScenarioReference(Guid runId, Guid scenarioId)
        {
            if (runId == Guid.Empty)
            {
                throw new ArgumentException("run id can not be empty", nameof(runId));
            }

            if (scenarioId == Guid.Empty)
            {
                throw new ArgumentException("scenario id can not be empty", nameof(scenarioId));
            }

            RunId = runId;
            ScenarioId = scenarioId;
        }

        public Guid RunId { get; }
        public Guid ScenarioId { get; }
    }
}
