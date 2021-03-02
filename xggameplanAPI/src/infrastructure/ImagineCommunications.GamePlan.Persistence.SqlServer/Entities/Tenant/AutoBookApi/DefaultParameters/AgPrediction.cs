using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    public class AgPrediction : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid AutoBookDefaultParametersId { get; set; }

        public int DemographicNo { get; set; }
        public int SalesAreaNo { get; set; }
        public double NoOfRtgs { get; set; }
    }
}
