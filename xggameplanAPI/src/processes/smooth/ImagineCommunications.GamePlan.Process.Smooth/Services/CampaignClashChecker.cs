using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Campaign clash checker
    /// </summary>
    public class CampaignClashChecker
        : ICampaignClashChecker
    {
        private readonly bool _enabled;

        public CampaignClashChecker(bool enabled) => _enabled = enabled;

        /// <summary>
        /// Returns spots that would have campaign clashes if the proposed spots
        /// were added.
        /// </summary>
        public List<Spot> GetCampaignClashesForNewSpots(
            IEnumerable<Spot> spots,
            IReadOnlyCollection<Spot> breakSpots)
        {
            if (!_enabled)
            {
                return new List<Spot>();
            }

            var spotClashes = new List<Spot>();

            foreach (var spot in spots)
            {
                IEnumerable<Spot> spotClashesForSpot;

                if (spot.IsMultipartSpot)
                {
                    spotClashesForSpot = SpotClashesForMultipartSpot(
                        spot,
                        breakSpots
                        );
                }
                else
                {
                    spotClashesForSpot = SpotClashesForSpot(
                        spot,
                        breakSpots
                        );
                }

                foreach (var spotClash in spotClashesForSpot)
                {
                    if (spotClashes.Contains(spotClash))
                    {
                        continue;
                    }

                    spotClashes.Add(spotClash);
                }
            }

            return spotClashes;
        }

        private IReadOnlyCollection<Spot> SpotClashesForMultipartSpot(
            Spot spot,
            IReadOnlyCollection<Spot> breakSpots)
        {
            IReadOnlyCollection<Spot> linkedMultipartSpots = BreakUtilities.GetLinkedMultipartSpots(
                spot,
                breakSpots,
                includeInputSpotInOutput: false);

            var externalSpotRefsToExclude = new List<string>(
                linkedMultipartSpots.Select(s => s.ExternalSpotRef));

            var spotClashesForSpot = breakSpots
                .Where(s => s.ExternalCampaignNumber == spot.ExternalCampaignNumber
                    && !externalSpotRefsToExclude.Contains(s.ExternalSpotRef))
                .ToList();

            return spotClashesForSpot;
        }

        private IReadOnlyCollection<Spot> SpotClashesForSpot(
            Spot spot,
            IReadOnlyCollection<Spot> breakSpots)
        {
            var spotClashesForSpot = breakSpots
                .Where(s => s.ExternalCampaignNumber == spot.ExternalCampaignNumber)
                .ToList();

            return spotClashesForSpot;
        }
    }
}
