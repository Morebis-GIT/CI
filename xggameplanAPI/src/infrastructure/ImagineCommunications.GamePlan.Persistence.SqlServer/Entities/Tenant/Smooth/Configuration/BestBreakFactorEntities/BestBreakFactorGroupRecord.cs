using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities
{
    public class BestBreakFactorGroupRecord: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SmoothConfigurationId { get; set; }

        public ICollection<BestBreakFactorGroupRecordPassSequenceItem> PassSequences { get; set; }

        public bool? SpotsCriteriaHasSponsoredSpots { get; set; }

        public bool? SpotsCriteriaHasFIBORLIBRequests { get; set; }

        public bool? SpotsCriteriaHasBreakRequest { get; set; }

        public BestBreakFactorGroup BestBreakFactorGroup { get; set; }
    }
}
