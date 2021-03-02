namespace xgCore.xgGamePlan.ApiEndPoints.Models.Runs
{
    public class KPIModel
    {
        public string Name { get; set; }
        public string DisplayFormat { get; set; }
        public double Value { get; set; }
        public KPIRanking Ranking { get; set; } = KPIRanking.None;
    }
    public enum KPIRanking
    {
        Best,
        SecondBest,
        None
    }
}
