using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;

namespace xggameplan.Model
{
    public class SponsoredItemModelBase
    {
        public IEnumerable<string> Products { get; set; }

        [Required]
        public SponsorshipCalculationType? CalculationType { get; set; }

        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
        public SponsorshipApplicability? Applicability { get; set; }
    }
}
