using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Results from a Smooth pass
    /// </summary>
    public class SmoothPassResult
    {
        public SmoothPassResult(int sequence) => Sequence = sequence;

        /// <summary>
        /// Smooth pass sequence
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Results of attempts to place spots. There will be one item for every
        /// call to PlaceSpots.
        /// </summary>
        public List<PlaceSpotsResult> PlaceSpotsResultList = new List<PlaceSpotsResult>();

        /// <summary>
        /// Booked spots that were unplaced due to restrictions
        /// </summary>
        public List<Guid> BookedSpotIdsUnplacedDueToRestrictions = new List<Guid>();

        /// <summary>
        /// Number of placed spots
        /// </summary>
        public int CountPlacedSpots
        {
            get
            {
                var spots = new HashSet<Guid>();

                foreach (var placeSpotsResult in PlaceSpotsResultList)
                {
                    foreach (var item in placeSpotsResult.PlacedSpotResults)
                    {
                        spots.AddDistinct(item.SpotId);
                    }
                }

                return spots.Count;
            }
        }

        /// <summary>
        /// Number of unplaced spots
        /// </summary>
        public int CountUnplacedSpots
        {
            get
            {
                var spots = new HashSet<Guid>();

                foreach (var placeSpotsResult in PlaceSpotsResultList)
                {
                    foreach (var item in placeSpotsResult.UnplacedSpotResults)
                    {
                        spots.AddDistinct(item.SpotId);
                    }
                }

                return spots.Count;
            }
        }
    }
}
