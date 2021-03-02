using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects
{
    public class RSDeliverySettings : ICloneable
    {
        public int DaysToCampaignEnd { get; set; }

        public int UpperLimitOfOverDelivery { get; set; }

        public int LowerLimitOfOverDelivery { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Returns whether the list of delivery settings
        /// </summary>
        /// <param name="demographicSettings1"></param>
        /// <param name="demographicSettings2"></param>
        /// <returns></returns>
        public static bool IsSame(List<RSDeliverySettings> deliverySettings1, List<RSDeliverySettings> deliverySettings2)
        {
            if (deliverySettings1 != null && deliverySettings2 != null && deliverySettings1.Count == deliverySettings2.Count)
            {
                System.Text.StringBuilder deliverySettingsString1 = new System.Text.StringBuilder("");
                deliverySettings1.OrderBy(ds => ds.DaysToCampaignEnd).ToList().ForEach(ds => deliverySettingsString1.Append(string.Format("#{0}", Serialize(ds))));
                System.Text.StringBuilder deliverySettingsString2 = new System.Text.StringBuilder("");
                deliverySettings2.OrderBy(ds => ds.DaysToCampaignEnd).ToList().ForEach(ds => deliverySettingsString2.Append(string.Format("#{0}", Serialize(ds))));
                return deliverySettingsString1.ToString() == deliverySettingsString2.ToString();
            }
            return false;
        }

        private static string Serialize(RSDeliverySettings deliverySettings)
        {
            return string.Format("DCE={1}{0}ULOD={2}{0}LLOD={3}", (Char)0, deliverySettings.DaysToCampaignEnd, deliverySettings.UpperLimitOfOverDelivery, deliverySettings.LowerLimitOfOverDelivery);
        }
    }
}
