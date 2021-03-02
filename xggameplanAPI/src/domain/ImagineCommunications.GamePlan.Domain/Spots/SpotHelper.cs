using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic;

namespace ImagineCommunications.GamePlan.Domain.Spots
{
    public static class SpotHelper
    {
        public static bool IsBooked(string externalBreakNo) =>
            !string.IsNullOrEmpty(externalBreakNo) &&
            !externalBreakNo.Equals(Globals.UnplacedBreakString, StringComparison.InvariantCultureIgnoreCase);

        public static Dictionary<int, Spot> IndexListById(IEnumerable<Spot> spots)
        {
            var indexedSpots = new Dictionary<int, Spot>();
            foreach (var spot in spots)
            {
                if (!indexedSpots.ContainsKey(spot.CustomId))
                {
                    indexedSpots.Add(spot.CustomId, spot);
                }
            }
            return indexedSpots;
        }

        public static Dictionary<Guid, Spot> IndexListByUid(IEnumerable<Spot> spots)
        {
            var indexedSpots = new Dictionary<Guid, Spot>();
            foreach (var spot in spots)
            {
                if (!indexedSpots.ContainsKey(spot.Uid))
                {
                    indexedSpots.Add(spot.Uid, spot);
                }
            }
            return indexedSpots;
        }
    }
}
