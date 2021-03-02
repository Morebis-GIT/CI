using System;

namespace xggameplan.Model
{
    public class CampaignPaybackModel : ICloneable
    {
        public string Name { get; set; }
        public double Amount { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
