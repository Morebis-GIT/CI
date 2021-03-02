using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults
{
    public class ScenarioCampaignMetric : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ScenarioId { get; set; }
        public string CampaignExternalId { get; set; }
        public int TotalSpots { get; set; }
        public int ZeroRatedSpots { get; set; }
        public double NominalValue { get; set; }
        public double TotalNominalValue { get; set; }
        public double DifferenceValueDelivered { get; set; }
        public double DifferenceValueDeliveredPercentage { get; set; }
    }
}
