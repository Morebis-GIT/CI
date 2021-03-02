using System;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    public class Rating
    {
        public DateTime Time { get; set; }
        public string Demographic { get; set; }
        public double NoOfRatings { get; set; }
    }
}
