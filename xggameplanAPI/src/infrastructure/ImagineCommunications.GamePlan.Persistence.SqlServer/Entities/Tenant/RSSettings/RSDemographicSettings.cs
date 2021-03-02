using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings
{
    public class RSDemographicSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int RSSettingId { get; set; }

        public string DemographicId { get; set; }

        public List<RSSettingsDemographicsDeliverySettings> DeliverySettingsList { get; set; } = new List<RSSettingsDemographicsDeliverySettings>();
    }
}
