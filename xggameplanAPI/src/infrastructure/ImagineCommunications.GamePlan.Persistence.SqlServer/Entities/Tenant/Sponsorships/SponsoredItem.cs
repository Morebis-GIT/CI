using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships
{
    public class SponsoredItem : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SponsorshipId { get; set; }
        public SponsorshipCalculationType CalculationType { get; set; }
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
        public SponsorshipApplicability? Applicability { get; set; }
        public ICollection<string> Products { get; set; }
        public ICollection<SponsorshipItem> SponsorshipItems { get; set; }
        public ICollection<SponsorshipClashExclusivity> ClashExclusivities { get; set; }
        public ICollection<SponsorshipAdvertiserExclusivity> AdvertiserExclusivities { get; set; }
    }
}
