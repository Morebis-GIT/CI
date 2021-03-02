namespace xggameplan.Model
{
    public class DurationFlattenModel
    {
        public int DurationSecs { get; set; }
        public decimal DurationTargetRatings { get; set; }
        public decimal DurationActualRatings { get; set; }
        public decimal DurationTargetActualDiff { get; set; }
        public decimal DurationTargetAchievedPct { get; set; }
    }
}
