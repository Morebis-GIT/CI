using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using xggameplan.Model;

namespace xggameplan.KPIProcessing
{
    public class CampaignProjectionProcessing
    {
        public static Dictionary<NodaTime.Duration, decimal> ProjectRatingsForCampaignDayPart(DayPartModel dayPart, IEnumerable<Recommendation> recommendations,
            DateTime startDate, DateTime endDate, List<string> salesAreaList)
        {
            Dictionary<NodaTime.Duration, decimal> lenRatingList = new Dictionary<NodaTime.Duration, decimal>();

            dayPart.Timeslices.ForEach(ts =>
            {
                ProjectedCampaignTimesliceChange(recommendations, startDate, endDate, ts, salesAreaList, ref lenRatingList);
            });

            return lenRatingList;
        }

        public static void ProjectedCampaignTimesliceChange(IEnumerable<Recommendation> recommendationCollection,
            DateTime startDate, DateTime endDate, TimesliceModel timeslice, List<string> salesAreaList, ref Dictionary<NodaTime.Duration, decimal> lenRatingList)
        {
            var timesliceRecommendations = recommendationCollection.Where(recommendation => timeslice.Contains(recommendation.StartDateTime, startDate, endDate) && salesAreaList.Contains(recommendation.SalesArea));

            //ignoring Action == 'A' as that is from smooth
            foreach (var ts in timesliceRecommendations)
            {
                if (ts.Action == "B")   //booked - add
                {
                    if (lenRatingList.ContainsKey(ts.SpotLength))
                    {
                        var sav = lenRatingList[ts.SpotLength];
                        lenRatingList.Remove(ts.SpotLength);
                        sav = sav + ts.SpotRating;
                        lenRatingList.Add(ts.SpotLength, sav);
                    }
                    else
                    {
                        lenRatingList.Add(ts.SpotLength, ts.SpotRating);
                    }
                }
                else if (ts.Action == "C")  //cancelled - subtract
                {
                    if (lenRatingList.ContainsKey(ts.SpotLength))
                    {
                        var sav = lenRatingList[ts.SpotLength];
                        lenRatingList.Remove(ts.SpotLength);
                        sav = sav - ts.SpotRating;
                        lenRatingList.Add(ts.SpotLength, sav);
                    }
                    else
                    {
                        lenRatingList.Add(ts.SpotLength, ts.SpotRating * -1); //make it a minus to subtract it
                    }
                }
            }
        }
    }
}
