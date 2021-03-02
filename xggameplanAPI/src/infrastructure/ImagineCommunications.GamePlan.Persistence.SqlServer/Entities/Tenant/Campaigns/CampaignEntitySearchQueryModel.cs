using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignEntitySearchQueryModel
    {
        public Campaign Campaign { get; set; }
        public Product Product { get; set; }
        public Demographic Demographic { get; set; }
        public Clash Clash { get; set; }
        public Advertiser Advertiser { get; set; }
        public Agency Agency { get; set; }
        public AgencyGroup AgencyGroup { get; set; }
        public Person Person { get; set; }
    }
}
