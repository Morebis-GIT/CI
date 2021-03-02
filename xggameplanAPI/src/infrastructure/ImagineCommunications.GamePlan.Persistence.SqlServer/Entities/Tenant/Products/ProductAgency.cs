using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products
{
    public class ProductAgency : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public int AgencyId { get; set; }
        public int? AgencyGroupId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Product Product { get; set; }
        public Agency Agency { get; set; }
        public AgencyGroup AgencyGroup { get; set; }
    }
}
