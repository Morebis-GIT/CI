using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships
{
    public class SponsorshipItemSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SponsorshipItemId { get; set; }

        public Guid SalesAreaId { get; set; }

        public SponsorshipItem SponsorshipItem { get; set; }

    }
}
