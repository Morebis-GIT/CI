using System;
using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class StrikeWeight
    {
        public StrikeWeight(DateTime startDate,
            DateTime endDate,
            decimal desiredPercentageSplit,
            decimal currentPercentageSplit,
            List<Length> lengths,
            List<DayPart> dayParts,
            int spotMaxRatings,
            double payback,
            double revenueBudget)
        {
            StartDate = startDate;
            EndDate = endDate;
            DesiredPercentageSplit = desiredPercentageSplit;
            CurrentPercentageSplit = currentPercentageSplit;
            Lengths = lengths;
            DayParts = dayParts;
            SpotMaxRatings = spotMaxRatings;
            Payback = payback;
            RevenueBudget = revenueBudget;
        }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public decimal DesiredPercentageSplit { get; }

        public decimal CurrentPercentageSplit { get; }

        public int SpotMaxRatings { get; }

        public double Payback { get; }

        public double RevenueBudget { get; }

        public List<Length> Lengths { get; }

        public List<DayPart> DayParts { get; }
    }
}
