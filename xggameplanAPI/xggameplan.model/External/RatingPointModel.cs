using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class RatingPointModel: ICloneable
    {
        public int Id { get; set; }
        public IEnumerable<string> SalesAreas { get; set; }
        public double? OffPeakValue { get; set; }
        public double? PeakValue { get; set; }
        public double? MidnightToDawnValue { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
