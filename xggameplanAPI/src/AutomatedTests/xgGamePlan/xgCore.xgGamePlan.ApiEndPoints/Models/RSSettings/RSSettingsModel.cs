using System;
using System.Collections.Generic;
using System.Linq;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.RSSettings
{
    public class RSSettingsModel : ICloneable
    {
        public int Id { get; set; }
        public string SalesArea { get; set; }

        public IEnumerable<RSDeliverySettingsModel> DefaultDeliverySettingsList { get; set; } = new List<RSDeliverySettingsModel>();

        public IEnumerable<RSDemographicSettingsModel> DemographicsSettings { get; set; } = new List<RSDemographicSettingsModel>();
        
        public object Clone()
        {
            var newObject = (RSSettingsModel)MemberwiseClone();
            newObject.DefaultDeliverySettingsList = DefaultDeliverySettingsList.Select(x => (RSDeliverySettingsModel)x.Clone());
            newObject.DemographicsSettings = DemographicsSettings.Select(x => (RSDemographicSettingsModel)x.Clone());
            return newObject;
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is RSSettingsModel other))
            {
                return false;
            }

            if (Id != other.Id)
            {
                return false;
            }

            if (SalesArea != other.SalesArea)
            {
                return false;
            }

            // Check sizes of lists
            if (DemographicsSettings.Count() != other.DemographicsSettings.Count() ||
                DefaultDeliverySettingsList.Count() != other.DefaultDeliverySettingsList.Count())
            {
                return false;
            }

            // Check collection items
            if (!DemographicsSettings.All(other.DemographicsSettings.Contains) ||
                !DefaultDeliverySettingsList.All(other.DefaultDeliverySettingsList.Contains))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = -1118611670;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SalesArea);
            return hashCode;
        }
    }
}
