using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings
{
    public class RSSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public Guid SalesAreaId { get; set; }

        public List<RSSettingsDefaultDeliverySettings> DefaultDeliverySettingsList { get; set; } = new List<RSSettingsDefaultDeliverySettings>();

        public List<RSDemographicSettings> DemographicsSettings { get; set; } = new List<RSDemographicSettings>();

        public SalesArea SalesArea { get; set; }
    }
}
