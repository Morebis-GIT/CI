using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;

namespace ImagineCommunications.GamePlan.Domain.Scenarios.Queries
{
    public class ScenarioDigestListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateUserModified { get; set; }
        public bool IsDefault { get; set; }
        public List<PassDigestListItem> Passes { get; set; }
    }
}
