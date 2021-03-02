using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using xggameplan.common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class FewestRequestedPositionInBreakRequestsFirstOrLastOnlyBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyCollection<Spot> _spots;

        public FewestRequestedPositionInBreakRequestsFirstOrLastOnlyBreakFactor(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots)
        {
            _smoothBreak = smoothBreak;
            _spots = spots;
        }

        public double StandardScore
        {
            get
            {
                // Break with fewest clashing first/last PIB requests has
                // highest score
                const double maxFactorScore = 1000;

                List<string> positionInBreakRequestsFirst = PositionInBreakRequests.GetPositionInBreakRequestsForSamePosition(PositionInBreakRequests.First);
                List<string> positionInBreakRequestsLast = PositionInBreakRequests.GetPositionInBreakRequestsForSamePosition(PositionInBreakRequests.Last);

                // For spots to add then determine if which have first/last PIB requests
                int countSpotsWithFirst = 0;
                int countSpotsWithLast = 0;

                foreach (var spot in _spots.Where(s => !String.IsNullOrEmpty(s.RequestedPositioninBreak)))
                {
                    if (positionInBreakRequestsFirst.Contains(spot.RequestedPositioninBreak))
                    {
                        countSpotsWithFirst++;
                    }
                    else if (positionInBreakRequestsLast.Contains(spot.RequestedPositioninBreak))
                    {
                        countSpotsWithLast++;
                    }
                }

                double factorScore = 0;

                // No spots to add have first/last PIB request, no clash so give
                // highest score
                if (countSpotsWithFirst == 0 && countSpotsWithLast == 0)
                {
                    factorScore = maxFactorScore;
                }
                else
                {
                    // Spots being added have first/last PIB request Count spots
                    // in break that have first/last position in break request,
                    // include booked spots too
                    int countSpotsInBreakWithFirst = 0;
                    int countSpotsInBreakWithLast = 0;

                    foreach (var spot in _smoothBreak.SmoothSpots)
                    {
                        if (!String.IsNullOrEmpty(spot.Spot.ActualPositioninBreak))
                        {
                            if (positionInBreakRequestsFirst.Contains(spot.Spot.ActualPositioninBreak) || spot.Spot.ActualPositioninBreak.Equals("1"))
                            {
                                countSpotsInBreakWithFirst++;
                            }

                            if (positionInBreakRequestsLast.Contains(spot.Spot.ActualPositioninBreak) || spot.Spot.ActualPositioninBreak.Equals("99"))     // TODO: Check this, don't think that they specify 99
                            {
                                countSpotsInBreakWithLast++;
                            }
                        }
                        else if (!String.IsNullOrEmpty(spot.Spot.RequestedPositioninBreak))
                        {
                            if (positionInBreakRequestsFirst.Contains(spot.Spot.RequestedPositioninBreak) || spot.Spot.RequestedPositioninBreak.Equals("1"))
                            {
                                countSpotsInBreakWithFirst++;
                            }

                            if (positionInBreakRequestsLast.Contains(spot.Spot.RequestedPositioninBreak) || spot.Spot.RequestedPositioninBreak.Equals("99"))    // TODO: Check this, don't think that they specify 99
                            {
                                countSpotsInBreakWithLast++;
                            }
                        }
                    }

                    // Determine factor score based on clashes. E.g. If not
                    // adding first PIB then we don't care if break has first
                    // PIB spots.
                    if (countSpotsWithFirst > 0 && countSpotsWithLast > 0)
                    {
                        factorScore = maxFactorScore - (countSpotsWithFirst + countSpotsWithLast);
                    }
                    else if (countSpotsWithFirst > 0 && countSpotsWithLast == 0)
                    {
                        factorScore = maxFactorScore - countSpotsWithFirst;
                    }
                    else if (countSpotsWithFirst == 0 && countSpotsWithLast > 0)
                    {
                        factorScore = maxFactorScore - countSpotsWithLast;
                    }
                }

                const double minStandardScore = 1;

                // Base should be 1
                factorScore = factorScore == 0 ? 1 : factorScore;

                return GetStandardFactorScore(
                    minStandardScore,
                    MaxStandardScore,
                    minStandardScore,
                    maxFactorScore,
                    factorScore);
            }
        }
    }
}
