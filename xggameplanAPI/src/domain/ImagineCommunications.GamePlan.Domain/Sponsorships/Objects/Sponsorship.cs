using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;

namespace ImagineCommunications.GamePlan.Domain.Sponsorships.Objects
{
    public class Sponsorship : EntityBase
    {
        public string ExternalReferenceId { get; set; }
        public SponsorshipRestrictionLevel RestrictionLevel { get; set; }
        public List<SponsoredItem> SponsoredItems { get; set; }
    }
}
