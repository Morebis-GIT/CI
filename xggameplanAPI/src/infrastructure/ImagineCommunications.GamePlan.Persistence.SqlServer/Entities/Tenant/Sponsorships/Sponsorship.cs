using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships
{
    public class Sponsorship : IAuditEntity, IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public string ExternalReferenceId { get; set; }
        public SponsorshipRestrictionLevel RestrictionLevel { get; set; }
        public ICollection<SponsoredItem> SponsoredItems { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
