using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas
{
    public class FunctionalAreaFaultType: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid FunctionalAreaId { get; set; }
        public int FaultTypeId { get; set; }

        public bool IsSelected { get; set; }

        public FaultType FaultType { get; set; }
    }
}
