using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products
{
    public class ProductAdvertiser : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public int AdvertiserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Product Product { get; set; }
        public Advertiser Advertiser { get; set; }
    }
}
