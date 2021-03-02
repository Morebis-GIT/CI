using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration
{
    public class SmoothDiagnosticConfiguration: IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int SmoothConfigurationId { get; set; }

        public ICollection<string> SpotSalesAreas { get; set; } = new List<string>(0);

        public ICollection<string> SpotDemographics { get; set; } = new List<string>(0);

        public ICollection<string> SpotExternalRefs { get; set; } = new List<string>(0);

        public ICollection<string> SpotExternalCampaignRefs { get; set; } = new List<string>(0);

        public ICollection<string> SpotMultipartSpots { get; set; } = new List<string>(0);

        public DateTime? SpotMinStartTime { get; set; }

        public DateTime? SpotMaxStartTime { get; set; }

        public int? SpotMinPreemptLevel { get; set; }

        public int? SpotMaxPreemptLevel { get; set; }
    }
}
