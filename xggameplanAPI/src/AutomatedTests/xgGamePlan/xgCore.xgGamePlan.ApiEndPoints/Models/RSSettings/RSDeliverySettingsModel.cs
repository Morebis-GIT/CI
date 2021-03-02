using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.RSSettings
{
    public class RSDeliverySettingsModel : ICloneable
    {
        public int DaysToCampaignEnd { get; set; }
        public int UpperLimitOfOverDelivery { get; set; }
        public int LowerLimitOfOverDelivery { get; set; }

        public object Clone() => (RSDeliverySettingsModel)MemberwiseClone();

        public override bool Equals(object obj) => obj is RSDeliverySettingsModel model &&
                                                   DaysToCampaignEnd == model.DaysToCampaignEnd &&
                                                   UpperLimitOfOverDelivery == model.UpperLimitOfOverDelivery &&
                                                   LowerLimitOfOverDelivery == model.LowerLimitOfOverDelivery;

        public override int GetHashCode()
        {
            int hashCode = 1383585845;
            hashCode = hashCode * -1521134295 + DaysToCampaignEnd.GetHashCode();
            hashCode = hashCode * -1521134295 + UpperLimitOfOverDelivery.GetHashCode();
            hashCode = hashCode * -1521134295 + LowerLimitOfOverDelivery.GetHashCode();
            return hashCode;
        }
    }
}
