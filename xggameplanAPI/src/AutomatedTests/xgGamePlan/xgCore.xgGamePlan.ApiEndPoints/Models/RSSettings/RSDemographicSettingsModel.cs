using System;
using System.Collections.Generic;
using System.Linq;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.RSSettings
{
    public class RSDemographicSettingsModel : ICloneable
    {
        public string DemographicId { get; set; }
        
        public IEnumerable<RSDeliverySettingsModel> DeliverySettingsList { get; set; } = new List<RSDeliverySettingsModel>();

        public object Clone()
        {
            var newObject = (RSDemographicSettingsModel)MemberwiseClone();
            newObject.DeliverySettingsList = DeliverySettingsList.Select(x => (RSDeliverySettingsModel)x.Clone());
            return newObject;
        }

        public override bool Equals(object obj) => obj is RSDemographicSettingsModel model &&
                                                   DemographicId == model.DemographicId &&
                                                   IsEqualList(model.DeliverySettingsList);

        public override int GetHashCode() => -1433810233 + EqualityComparer<string>.Default.GetHashCode(DemographicId);

        private bool IsEqualList(IEnumerable<RSDeliverySettingsModel> list)
        {
            if (list == null || DeliverySettingsList.Count() != list.Count())
            {
                return false;
            }

            return DeliverySettingsList.All(list.Contains);
        }
    }
}
