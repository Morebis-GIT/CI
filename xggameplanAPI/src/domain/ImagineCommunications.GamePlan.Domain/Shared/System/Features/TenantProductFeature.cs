using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Features
{
    public class TenantProductFeature : IFeatureSetting
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public bool IsShared { get; set; }

        public List<int> ParentIds { get; set; } = new List<int>();

        IReadOnlyCollection<int> IFeatureSetting.ParentIds => ParentIds?.ToArray() ?? Array.Empty<int>();
    }
}
