using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class AgencyGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Code { get; set; }
    }
}
