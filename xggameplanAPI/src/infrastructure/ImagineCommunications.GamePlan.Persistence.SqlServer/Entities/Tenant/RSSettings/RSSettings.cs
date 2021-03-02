using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings
{
    public class RSSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public string SalesArea { get; set; }

        public List<RSSettingsDefaultDeliverySettings> DefaultDeliverySettingsList { get; set; } = new List<RSSettingsDefaultDeliverySettings>();

        public List<RSDemographicSettings> DemographicsSettings { get; set; } = new List<RSDemographicSettings>();
    }
}
