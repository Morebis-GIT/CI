using System;

namespace ImagineCommunications.GamePlan.Domain.Scenarios.Objects
{
    public class ScenariosWithPassIdTransformerResult
    {
        public string Id { get; set; }

        public Guid ScenarioId => new Guid(Id.Split('/')[1]);

        public int PassId { get; set; }
    }
}
