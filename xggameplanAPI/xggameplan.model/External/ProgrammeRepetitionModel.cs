using System;

namespace xggameplan.Model
{
    public class ProgrammeRepetitionModel : ICloneable
    {
        public int Minutes { get; set; }
        public double Factor { get; set; }
        public double? PeakFactor { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
