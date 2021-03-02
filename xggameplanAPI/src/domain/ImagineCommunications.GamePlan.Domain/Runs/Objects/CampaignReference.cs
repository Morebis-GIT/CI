using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class CampaignReference : ICloneable
    {
        public string ExternalId { get; set; }     

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
