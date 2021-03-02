using System;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using xggameplan.Common;

namespace xggameplan.Autopilot
{
    public static class AutopilotRuleExtensions
    {
        public static int GetAdjustmentValue(this AutopilotRule rule, AutopilotPassType passType)
        {
            if (rule == null)
            {
                return 0;
            }

            switch (passType)
            {
                case AutopilotPassType.TightenALot:
                    return rule.TightenLot;
                case AutopilotPassType.TightenABit:
                    return rule.TightenBit;
                case AutopilotPassType.LoosenABit:
                    return rule.LoosenBit;
                case AutopilotPassType.LoosenALot:
                    return rule.LoosenLot;
                default:
                    throw new ArgumentOutOfRangeException(nameof(passType), passType, "Unknown pass type provided");
            }
        }
    }
}
