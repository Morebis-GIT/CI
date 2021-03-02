using System.Collections.Generic;

namespace xggameplan.Model
{
    public class RSDemographicSettingsModel
    {
        public string DemographicId { get; set; }

        public List<RSDeliverySettingsModel> DeliverySettingsList = new List<RSDeliverySettingsModel>();
    }
}
