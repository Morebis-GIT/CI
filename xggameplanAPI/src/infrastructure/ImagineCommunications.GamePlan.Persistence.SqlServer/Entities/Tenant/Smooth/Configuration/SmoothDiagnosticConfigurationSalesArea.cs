using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration
{
    public class SmoothDiagnosticConfigurationSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int SmoothDiagnosticConfigurationId { get; set; }

        public SmoothDiagnosticConfiguration SmoothDiagnosticConfiguration { get; set; }
        public Guid SalesAreaId { get; set; }
    }
}
