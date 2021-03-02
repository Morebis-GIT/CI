using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas
{
    public class FunctionalArea : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }

        public ICollection<FunctionalAreaFaultType> FunctionalAreaFaultTypes { get; set; } =
            new List<FunctionalAreaFaultType>(0);

        public ICollection<FunctionalAreaDescription> Descriptions { get; set; } =
            new List<FunctionalAreaDescription>(0);
    }
}
