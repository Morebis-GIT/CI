using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships
{
    public class SponsorshipClashExclusivity : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SponsoredItemId { get; set; }
        public string ClashExternalRef { get; set; }
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
    }
}
