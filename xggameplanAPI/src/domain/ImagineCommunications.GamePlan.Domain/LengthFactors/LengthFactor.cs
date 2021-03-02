using System;

namespace ImagineCommunications.GamePlan.Domain.LengthFactors
{
    public class LengthFactor
    {
        public int Id { get; set; }
        public string SalesArea { get; set; }
        public TimeSpan Duration { get; set; }
        public double Factor { get; set; }
    }
}
