using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects
{
    public class RSDemographicSettings : ICloneable
    {
        public string DemographicId { get; set; }

        public List<RSDeliverySettings> DeliverySettingsList = new List<RSDeliverySettings>();

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Returns whether the list of demographic settings are the same
        /// </summary>
        /// <param name="demographicSettings1"></param>
        /// <param name="demographicSettings2"></param>
        /// <returns></returns>
        public static bool IsSame(List<RSDemographicSettings> demographicSettingsList1, List<RSDemographicSettings> demographicSettingsList2)
        {
            if (demographicSettingsList1 != null && demographicSettingsList2 != null && demographicSettingsList1.Count == demographicSettingsList2.Count &&
               (demographicSettingsList1.Select(ds => ds.DemographicId).Distinct().Count() == demographicSettingsList2.Select(ds => ds.DemographicId).Distinct().Count()))
            {
                // Compare each demographic settings
                foreach (var demographicSettings1 in demographicSettingsList1)
                {
                    var demographicSettings2 = demographicSettingsList2.Where(ds => ds.DemographicId == demographicSettings1.DemographicId).FirstOrDefault();
                    if (demographicSettings2 == null || !demographicSettings1.IsSame(demographicSettings2))
                    {
                        return false;
                    }
                }
                return true;

                /*
                System.Text.StringBuilder demographicString1 = new System.Text.StringBuilder("");
                demographicSettingsList1.OrderBy(ds => ds.DemographicId).ToList().ForEach(ds => demographicString1.Append(string.Format("#{0}", Serialize(ds))));
                System.Text.StringBuilder demographicString2 = new System.Text.StringBuilder("");
                demographicSettingsList2.OrderBy(ds => ds.DemographicId).ToList().ForEach(ds => demographicString2.Append(string.Format("#{0}", Serialize(ds))));
                if (demographicString1.ToString() == demographicString2.ToString())
                {
                    // Check delivery settings
                    foreach(var demographicSettings1 in demographicSettingsList1)
                    {
                        var demographicSettings2 = demographicSettingsList2.Where(ds => ds.DemographicId == demographicSettings1.DemographicId).FirstOrDefault();
                        if (demographicSettings2 == null)
                        {
                            return false;
                        }
                        else if (!RightSizerDeliverySettings.IsSame(demographicSettings1.DeliverySettingsList, demographicSettings2.DeliverySettingsList))
                        {
                            return false;
                        }
                    }
                }
                */
            }
            return false;
        }

        public bool IsSame(RSDemographicSettings rsSettingsDemographicSettings)
        {
            if (DemographicId == rsSettingsDemographicSettings.DemographicId)
            {
                return RSDeliverySettings.IsSame(this.DeliverySettingsList, rsSettingsDemographicSettings.DeliverySettingsList);
            }
            return false;
        }

        //private static string Serialize(RightSizerDemographicSettings demographicSettings)
        //{
        //    return string.Format("ID={1}{0}ET={2}", (Char)0, demographicSettings.DemographicId, "XXX");
        //}
    }
}
