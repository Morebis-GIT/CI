using System;

namespace xggameplan.Model
{
    public class StrikeWeightFlattenModel
    {
        public DateTime StrikeWeightStartDate { get; set; }
        public DateTime StrikeWeightEndDate { get; set; }
        public decimal StrikeWeightTargetRatings { get; set; }
        public decimal StrikeWeightActualRatings { get; set; }
        public decimal StrikeWeightTargetActualDiff { get; set; }
        public decimal StrikeWeightTargetAchievedPct { get; set; }
    }
}
