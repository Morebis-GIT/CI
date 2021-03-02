using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas
{
    public class FaultType
    {
        public int Id { get; set; }

        public FunctionalAreaFaultType FunctionalAreaFaultType { get; set; }
        public ICollection<FaultTypeDescription> Descriptions { get; set; } = new List<FaultTypeDescription>(0);
    }
}
