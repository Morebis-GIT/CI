using System;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;

namespace ImagineCommunications.GamePlan.Domain.Sponsorships.Objects
{
    public class AdvertiserExclusivity : ICloneable
    {
        public string AdvertiserIdentifier { get; set; }
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
