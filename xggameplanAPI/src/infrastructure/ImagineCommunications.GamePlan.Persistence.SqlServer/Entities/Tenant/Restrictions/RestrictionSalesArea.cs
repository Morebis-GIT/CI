using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions
{
    public class RestrictionSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int RestrictionId { get; set; }
        public string SalesArea { get; set; }
    }
}
