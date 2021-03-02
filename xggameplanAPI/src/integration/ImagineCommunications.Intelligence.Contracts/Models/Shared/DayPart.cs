using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class DayPart
    {
        public DayPart(
            decimal desiredPercentageSplit,
            decimal currentPercentageSplit,
            List<Timeslice> timeslices,
            List<DayPartLength> lengths,
            int spotMaxRatings,
            decimal campaignPrice,
            int totalSpotCount,
            int zeroRatedSpotCount,
            double ratings,
            double baseDemographRatings,
            double nominalValue,
            double payback,
            double revenueBudget)
        {
            DesiredPercentageSplit = desiredPercentageSplit;
            CurrentPercentageSplit = currentPercentageSplit;
            Timeslices = timeslices;
            Lengths = lengths;
            SpotMaxRatings = spotMaxRatings;
            CampaignPrice = campaignPrice;
            TotalSpotCount = totalSpotCount;
            ZeroRatedSpotCount = zeroRatedSpotCount;
            Ratings = ratings;
            BaseDemographRatings = baseDemographRatings;
            NominalValue = nominalValue;
            RevenueBudget = revenueBudget;
            Payback = payback;
        }

        public decimal DesiredPercentageSplit { get; }
        public decimal CurrentPercentageSplit { get; }
        public int SpotMaxRatings { get; }

        public List<Timeslice> Timeslices { get; }

        public List<DayPartLength> Lengths { get; }

        public decimal CampaignPrice { get; }
        public int TotalSpotCount { get; }
        public int ZeroRatedSpotCount { get; }
        public double Ratings { get; }
        public double BaseDemographRatings { get; }
        public double NominalValue { get; }
        public double RevenueBudget { get; }
        public double Payback { get; }
    }
}
