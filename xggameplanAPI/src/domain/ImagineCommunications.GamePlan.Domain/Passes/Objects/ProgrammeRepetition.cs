using System;

namespace ImagineCommunications.GamePlan.Domain.Passes.Objects
{
    public class ProgrammeRepetition : ICloneable
    {
        public int Minutes { get; set; }

        public double Factor { get; set; }

        public double? PeakFactor { get; set; }

        public object Clone()
        {

            return this.MemberwiseClone();
        }
    }
}
