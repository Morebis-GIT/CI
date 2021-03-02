namespace xggameplan.Model
{
    public class DayPartFlattenModel
    {
        public string DaypartTimeAndDays { get; set; }
        public decimal DaypartTargetRatings { get; set; }
        public decimal DaypartActualRatings { get; set; }
        public decimal DaypartTargetActualDiff { get; set; }
        public decimal DaypartTargetAchievedPct { get; set; }
    }
}
