using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.LengthFactor
{
    public class LengthFactorCreated : ILengthFactorCreated
    {
        public string SalesArea { get; }
        public TimeSpan Duration { get; }
        public double Factor { get; }

        public LengthFactorCreated(string salesArea, TimeSpan duration, double factor)
        {
            SalesArea = salesArea;
            Duration = duration;
            Factor = factor;
        }
    }
}
