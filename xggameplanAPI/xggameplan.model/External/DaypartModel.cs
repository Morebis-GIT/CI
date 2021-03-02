using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class DayPartModel : ICloneable
    {
        public string DayPartName { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

        public int ScenarioRecommendationCurrentPercentageSplit { get; set; }

        public List<TimesliceModel> Timeslices { get; set; }

        public List<DayPartLengthModel> Lengths { get; set; }

        public int SpotMaxRatings { get; set; }
        public double? Payback { get; set; }
        public double? RevenueBudget { get; set; }

        public object Clone()
        {
            DayPartModel dayPartModel = (DayPartModel)this.MemberwiseClone();

            if (this.Timeslices != null)
            {
                dayPartModel.Timeslices = new List<TimesliceModel>();
                this.Timeslices.ForEach(ts => dayPartModel.Timeslices.Add((TimesliceModel)ts.Clone()));
            }

            return dayPartModel;
        }
    }
}
