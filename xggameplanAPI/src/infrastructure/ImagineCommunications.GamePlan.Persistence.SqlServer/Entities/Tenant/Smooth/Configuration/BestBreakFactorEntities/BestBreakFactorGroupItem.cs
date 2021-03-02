using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities
{
    public class BestBreakFactorGroupItem: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int BestBreakFactorGroupId { get; set; }

        //Navigation THP with discriminator instead?
        public ICollection<BestBreakDefaultFactor> DefaultFactors { get; set; }

        //Navigation
        public ICollection<BestBreakFilterFactor> FilterFactors { get; set; }

        public bool AllFilterFactorsMustBeNonZero { get; set; }

        public bool UseZeroScoresInEvaluation { get; set; }

        public BestBreakFactorItemEvaluation Evaluation { get; set; }
    }
}
