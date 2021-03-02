using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace xggameplan.Model
{
    public class StrikeWeightModel : ICloneable
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public List<Length> Lengths { get; set; }
        public List<DayPartModel> DayParts { get; set; }
        public int SpotMaxRatings { get; set; }
        public double? Payback { get; set; }
        public double? RevenueBudget { get; set; }

        public object Clone()
        {
            StrikeWeightModel strikeWeightModel = (StrikeWeightModel)this.MemberwiseClone();

            if (this.Lengths != null)
            {
                strikeWeightModel.Lengths = new List<Length>();
                this.Lengths.ForEach(le => strikeWeightModel.Lengths.Add((Length)le.Clone()));
            }
            if (this.DayParts != null)
            {
                strikeWeightModel.DayParts = new List<DayPartModel>();
                this.DayParts.ForEach(dp => strikeWeightModel.DayParts.Add((DayPartModel)dp.Clone()));
            }

            return strikeWeightModel;
        }
    }
}
