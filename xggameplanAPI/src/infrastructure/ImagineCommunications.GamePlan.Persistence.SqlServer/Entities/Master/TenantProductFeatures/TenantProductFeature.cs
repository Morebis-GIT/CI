using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures
{
    public class TenantProductFeature : IIdentityPrimaryKey, IFeatureSetting
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public bool IsShared { get; set; }

        public ICollection<TenantProductFeatureReference> ParentFeatures { get; set; } = new HashSet<TenantProductFeatureReference>();

        IReadOnlyCollection<int> IFeatureSetting.ParentIds =>
            ParentFeatures?.Select(x => x.TenantProductFeatureParentId).ToArray() ?? Array.Empty<int>();
    }
}
