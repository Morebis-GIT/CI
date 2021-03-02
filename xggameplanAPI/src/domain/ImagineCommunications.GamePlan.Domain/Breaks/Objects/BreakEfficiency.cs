namespace ImagineCommunications.GamePlan.Domain.Breaks.Objects
{
    public class BreakEfficiency
    {
        public BreakEfficiency(string demographic, double efficiency)
        {
            Demographic = demographic;
            Efficiency = efficiency;
        }

        public string Demographic { get; set; }
        public double Efficiency { get; set; }
    }
}
