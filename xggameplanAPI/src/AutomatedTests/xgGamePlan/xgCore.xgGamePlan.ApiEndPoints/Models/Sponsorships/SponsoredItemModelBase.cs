using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public abstract class SponsoredItemModelBase
    {
        public IEnumerable<string> Products { get; set; }
        public SponsorshipCalculationType CalculationType { get; set; }
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
        public SponsorshipApplicability? Applicability { get; set; }
    }
}
