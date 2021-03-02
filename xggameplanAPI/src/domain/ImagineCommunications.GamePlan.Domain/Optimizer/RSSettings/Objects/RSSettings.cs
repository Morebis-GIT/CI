using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects
{
    /// <summary>
    /// Right Sizer settings
    /// </summary>
    public class RSSettings : ICloneable
    {
        public int Id { get; set; }

        public string SalesArea { get; set; }

        public List<RSDeliverySettings> DefaultDeliverySettingsList = new List<RSDeliverySettings>();

        public List<RSDemographicSettings> DemographicsSettings = new List<RSDemographicSettings>();

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Updates from other RSSettings instances, leaves Id & SalesArea
        /// Update Mode:
        /// 0=Current sales area only (include demographics) - Not passed through
        /// 1=All sales areas (include demographics)
        /// 2=All sales areas (exclude demographics)
        /// 3=All sales areas (Demographics only)
        /// </summary>
        /// <param name="isrSettings"></param>
        /// <param name="updateMode"></param>
        public void UpdateFrom(RSSettings rsSettings, int updateMode)
        {
            // Update main settings
            if (updateMode == 1 || updateMode == 2)
            {
                DefaultDeliverySettingsList.Clear();
                foreach (var deliverySettings in rsSettings.DefaultDeliverySettingsList)
                {
                    DefaultDeliverySettingsList.Add((RSDeliverySettings)deliverySettings.Clone());
                }
            }

            // Update demographics
            if (updateMode == 1 || updateMode == 3)
            {
                DemographicsSettings.Clear();
                foreach (var demographicSettings in rsSettings.DemographicsSettings)
                {
                    var newDemographicSettings = new RSDemographicSettings() { DemographicId = demographicSettings.DemographicId };
                    demographicSettings.DeliverySettingsList.ForEach(ds => newDemographicSettings.DeliverySettingsList.Add((RSDeliverySettings)ds.Clone()));
                    DemographicsSettings.Add(newDemographicSettings);
                }
            }
        }

        /// <summary>
        /// Returns whether instance has same settings as input
        /// Compare Mode:
        /// 0=Full settings comparison (include demographics)
        /// 1=Top level settings comparison (exclude demographics)
        /// 2=Demographic settings only
        /// </summary>
        /// <param name="isrSettings"></param>
        /// <param name="compareMode"></param>
        /// <returns></returns>
        public bool IsSame(RSSettings rsSettings, int compareMode)
        {
            // Compare main settings
            if (compareMode == 0 || compareMode == 1)
            {
                if (!RSDeliverySettings.IsSame(DefaultDeliverySettingsList, rsSettings.DefaultDeliverySettingsList))
                {
                    return false;
                }
            }

            // Compare demographics
            if (compareMode == 0 || compareMode == 2)
            {
                return RSDemographicSettings.IsSame(DemographicsSettings, rsSettings.DemographicsSettings);
            }
            return true;
        }

        public static void ValidateForSave(RSSettings rsSettings)
        {
            if (String.IsNullOrEmpty(rsSettings.SalesArea))
            {
                throw new Exception("Sales Area is not set");
            }

            // Check default settings
            if (rsSettings.DefaultDeliverySettingsList.FindAll(ds => ds.DaysToCampaignEnd < 0).Count > 0)
            {
                throw new Exception("Days To Campaign End for default delivery settings cannot be less than zero");
            }
            if (rsSettings.DefaultDeliverySettingsList.FindAll(ds => ds.LowerLimitOfOverDelivery < 0).Count > 0)
            {
                throw new Exception("Lower Limit of Over Delivery for default delivery settings cannot be less than zero");
            }
            if (rsSettings.DefaultDeliverySettingsList.FindAll(ds => ds.UpperLimitOfOverDelivery < 0).Count > 0)
            {
                throw new Exception("Upper Limit of Over Delivery for default delivery settings cannot be less than zero");
            }
            if (rsSettings.DefaultDeliverySettingsList.FindAll(ds => ds.UpperLimitOfOverDelivery < ds.LowerLimitOfOverDelivery).Count > 0)
            {
                throw new Exception("Lower Limit of Over Delivery for default delivery settings must be less than Upper Limit of Over Delivery");
            }
            /*
            if (rsSettings.DefaultDeliverySettingsList.FindAll(ds => ds.UpperLimitOfOverDelivery == ds.LowerLimitOfOverDelivery).Count > 0)
            {
                throw new Exception("Lower Limit of Over Delivery for default deliver settings cannot be the same as Upper Limit of Over Delivery");
            }
            */

            // Check demographic settings
            if (rsSettings.DemographicsSettings.FindAll(ds => ds.DeliverySettingsList.Where(s => s.DaysToCampaignEnd < 0).Any()).Count > 0)
            {
                throw new Exception("Days To Campaign End for demographic delivery settings cannot be less than zero");
            }
            if (rsSettings.DemographicsSettings.FindAll(ds => ds.DeliverySettingsList.Where(s => s.LowerLimitOfOverDelivery < 0).Any()).Count > 0)
            {
                throw new Exception("Lower Limit of Over Delivery for demographic delivery settings cannot be less than zero");
            }
            if (rsSettings.DemographicsSettings.FindAll(ds => ds.DeliverySettingsList.Where(s => s.UpperLimitOfOverDelivery < 0).Any()).Count > 0)
            {
                throw new Exception("Upper Limit of Over Delivery for demographic delivery settings cannot be less than zero");
            }
            if (rsSettings.DemographicsSettings.FindAll(ds => ds.DeliverySettingsList.Where(s => s.UpperLimitOfOverDelivery < s.LowerLimitOfOverDelivery).Any()).Count > 0)
            {
                throw new Exception("Lower Limit of Over Delivery for demographic delivery setting must be less than Upper Limit of Over Delivery");
            }
            /*
            if (rsSettings.DemographicsSettings.FindAll(ds => ds.DeliverySettingsList.Where(s => s.UpperLimitOfOverDelivery == s.LowerLimitOfOverDelivery).Any()).Count > 0)
            {
                throw new Exception("Lower Limit of Over Delivery for demographic delivery setting cannot be the same as Upper Limit of Over Delivery");
            }
            */
        }
    }
}
