using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class EfficiencySettings : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }
        public int EfficiencyCalculationPeriod { get; set; }
        public int? DefaultNumberOfWeeks { get; set; }
        public int PersistEfficiency { get; set; }
    }
}
