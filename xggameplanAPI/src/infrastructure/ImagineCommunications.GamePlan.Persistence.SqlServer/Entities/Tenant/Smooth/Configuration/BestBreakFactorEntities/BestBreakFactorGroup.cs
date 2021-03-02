using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities
{
    public class BestBreakFactorGroup: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int BestBreakFactorGroupRecordId { get; set; }

        //Navigation
        public ICollection<BestBreakFactorGroupItem> Items { get; set; }

        public BestBreakFactorGroupEvaluation Evaluation { get; set; }

        public SameBreakGroupScoreActions SameBreakGroupScoreAction { get; set; }

        //Navigation
        public SameBreakGroupScoreFactor SameBreakGroupScoreFactor { get; set; }

        public int Sequence { get; set; }

        public string Name { get; set; }
    }
}
