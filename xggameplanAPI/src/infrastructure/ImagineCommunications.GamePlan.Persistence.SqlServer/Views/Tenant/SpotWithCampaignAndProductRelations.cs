using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant
{
    public class SpotWithCampaignAndProductRelations
    {
        public Guid SpotUid { get; set; }
        public string CampaignExternalId { get; set; }
        public Guid? ProductId { get; set; }
        public int? ProductAdvertiserId { get; set; }
        public int? ProductAgencyId { get; set; }
        public int? ProductPersonId { get; set; }
    }
}
