using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class RunsWithScenarioIdTransformerResult
    {
        public string Id { get; set; }

        public Guid RunId => new Guid(Id.Split('/')[1]);

        public Guid ScenarioId { get; set; }
    }
}
