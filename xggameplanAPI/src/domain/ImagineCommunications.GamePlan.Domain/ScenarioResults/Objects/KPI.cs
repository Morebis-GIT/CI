namespace ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects
{
    public class KPI
    {

        public string Name { get; set; }
        public string Displayformat { get; set; }
        public double Value { get; set; }
        public KPISource ResultSource { get; set; }
    }
}
