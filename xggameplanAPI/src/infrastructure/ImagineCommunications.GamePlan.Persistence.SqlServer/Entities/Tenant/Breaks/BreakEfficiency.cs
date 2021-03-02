using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks
{
    public class BreakEfficiency : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid BreakId { get; set; }
        public string Demographic { get; set; }
        public double Efficiency { get; set; }
    }
}
