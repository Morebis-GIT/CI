using System.Collections.Generic;

namespace xggameplan.Model
{
    public class RSSettingsModel
    {
        public int Id { get; set; }

        public string SalesArea { get; set; }

        public List<RSDeliverySettingsModel> DefaultDeliverySettingsList = new List<RSDeliverySettingsModel>();

        public List<RSDemographicSettingsModel> DemographicsSettings = new List<RSDemographicSettingsModel>();
    }
}
