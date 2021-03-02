using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTimeRestriction : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid CampaignId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public IncludeOrExclude IsIncludeOrExclude { get; set; }
        public SortedSet<DayOfWeek> DowPattern { get; set; } = new SortedSet<DayOfWeek>();

        public ICollection<CampaignTimeRestrictionSalesArea> SalesAreas { get; set; } =
            new HashSet<CampaignTimeRestrictionSalesArea>();
    }
}
