using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas
{
    public class SalesAreasChannelGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid SalesAreaId { get; set; }
        public string Name { get; set; }
    }
}
