using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignSalesAreaTargetMultipart : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignSalesAreaTargetId { get; set; }
        public int MultipartNumber { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

        public ICollection<CampaignSalesAreaTargetMultipartLength> Lengths { get; set; } =
            new HashSet<CampaignSalesAreaTargetMultipartLength>();
    }
}
