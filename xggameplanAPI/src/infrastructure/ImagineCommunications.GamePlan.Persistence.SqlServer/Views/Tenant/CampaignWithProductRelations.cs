using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant
{
    public class CampaignWithProductRelations
    {
        public Guid CampaignId { get; set; }
        public Guid? ProductId { get; set; }
        public int? AdvertiserId { get; set; }
        public int? AgencyId { get; set; }
        public int? AgencyGroupId { get; set; }
        public int? PersonId { get; set; }
    }
}
