using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignSalesAreaTarget : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid CampaignId { get; set; }
        public Guid SalesAreaId { get; set; }
        public bool StopBooking { get; set; }

        public CampaignSalesAreaTargetGroup SalesAreaGroup { get; set; }

        public ICollection<CampaignSalesAreaTargetMultipart> Multiparts { get; set; } =
            new HashSet<CampaignSalesAreaTargetMultipart>();
        public ICollection<CampaignTarget> CampaignTargets { get; set; } =
            new HashSet<CampaignTarget>();

        public SalesArea SalesArea { get; set; }
    }
}
