using System;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Result of attempt to determine if spot can be placed in break.
    /// </summary>
    public readonly struct PlaceSpotResult
    {
        public PlaceSpotResult(Guid spotUid)
        {
            SpotId = spotUid;
            ExternalBreakRef = null;
            ValidateAddSpotsResultForSpot = new SmoothFailuresForSpotCollection(spotUid);
        }

        public PlaceSpotResult(
            Guid spotUid,
            string externalBreakRef)
            : this(spotUid)
        {
            ExternalBreakRef = externalBreakRef;
        }

        public PlaceSpotResult(
            Guid spotUid,
            string externalBreakRef,
            SmoothFailuresForSpotCollection validateAddSpotsResultForSpot
            )
            : this(spotUid, externalBreakRef)
        {
            ValidateAddSpotsResultForSpot = validateAddSpotsResultForSpot;
        }

        /// <summary>
        /// The Id of the placed <see cref="Spot"/>
        /// </summary>
        public Guid SpotId { get; }

        /// <summary>
        /// The external reference of the Break that spot was attempted to be
        /// placed in
        /// </summary>
        public string ExternalBreakRef { get; }

        /// <summary>
        /// Initialised to an empty list.
        /// </summary>
        public SmoothFailuresForSpotCollection ValidateAddSpotsResultForSpot { get; }
    }
}
