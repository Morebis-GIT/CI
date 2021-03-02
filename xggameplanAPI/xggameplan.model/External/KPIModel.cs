using ImagineCommunications.GamePlan.Domain;

namespace xggameplan.Model
{
    public enum KPIRanking
    {
        Best, SecondBest, None
    }

    public class KPIModel
    {
        public string Name { get; set; }
        public string DisplayFormat { get; set; }
        public double Value { get; set; }
        public KPIRanking Ranking { get; set; } = KPIRanking.None;
        public KPISource ResultSource { get; set; }
    }
}
