using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTimeRestrictionSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignTimeRestrictionId { get; set; }
        public Guid SalesAreaId { get; set; }

        public SalesArea SalesArea { get; set; }
    }
}
