using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using NodaTime;
using xggameplan.common;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Details of a Break within Smooth. Contains the Break plus Smooth specific properites.
    /// </summary>
    [DebuggerDisplay(nameof(Break.ExternalBreakRef) + " = {" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class SmoothBreak
        : ICloneable
    {
        private string GetDebuggerDisplay() => TheBreak.ExternalBreakRef;

        private static readonly int _breakSequenceForMultipartTopTailAndTopTailTop =
            SpotPositioning.GetBreakSeqFromMultipartSpotPosition(
                MultipartSpotTypes.TopTail,
                MultipartSpotPositions.TopTail_Top);

        private static readonly int _breakSequenceForMultipartTopTailAndTopTailTail =
            SpotPositioning.GetBreakSeqFromMultipartSpotPosition(
                MultipartSpotTypes.TopTail,
                MultipartSpotPositions.TopTail_Tail);

        private static readonly int _breakSequenceForMultipartSameBreakAndSameBreakTop =
            SpotPositioning.GetBreakSeqFromMultipartSpotPosition(
                MultipartSpotTypes.SameBreak,
                MultipartSpotPositions.SameBreak_Top);

        private static readonly int _breakSequenceForMultipartSameBreakAndSameBreakTail =
            SpotPositioning.GetBreakSeqFromMultipartSpotPosition(
                MultipartSpotTypes.SameBreak,
                MultipartSpotPositions.SameBreak_Tail);

        private static readonly int _breakSequenceForMultipartSameBreakAndSameBreakMiddle =
            SpotPositioning.GetBreakSeqFromMultipartSpotPosition(
                MultipartSpotTypes.SameBreak,
                MultipartSpotPositions.SameBreak_Mid);

        private static readonly int _breakSequenceForMultipartSameBreakAndSameBreakAny =
            SpotPositioning.GetBreakSeqFromMultipartSpotPosition(
                MultipartSpotTypes.SameBreak,
                MultipartSpotPositions.SameBreak_Any);

        public SmoothBreak(Break theBreak, int position)
        {
            TheBreak = theBreak;
            Position = position;
            RemainingAvailability = TheBreak.Avail;
        }

        public SmoothBreak(Break theBreak, int position, List<SmoothSpot> smoothSpots)
            : this(theBreak, position) => SmoothSpots = smoothSpots;

        public Break TheBreak { get; }

        /// <summary>
        /// Spots for break
        /// </summary>
        public List<SmoothSpot> SmoothSpots { get; } = new List<SmoothSpot>();

        /// <summary>
        /// The remaining time availiabilty of the break.
        /// </summary>
        public Duration RemainingAvailability { get; set; }

        /// <summary>
        /// Break position within the programme. For example, 1 is the first break,
        /// 2 is the second break and so on.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// The SmoothProgramme instance this SmoothBreak belongs to.
        /// </summary>
        public SmoothProgramme SmoothProgramme { get; set; }

        public object Clone()
        {
            var smoothSpots = new List<SmoothSpot>();

            foreach (var smoothSpot in SmoothSpots)
            {
                smoothSpots.Add(
                    (SmoothSpot)smoothSpot.Clone()
                    );
            }

            var breakClone = TheBreak is null
                ? null
                : (Break)TheBreak.Clone();

            var result = new SmoothBreak(breakClone, Position, smoothSpots)
            {
                RemainingAvailability = RemainingAvailability,
                SmoothProgramme = SmoothProgramme is null
                    ? null
                    : (SmoothProgramme)SmoothProgramme.Clone()
            };

            return result;
        }

        /// <summary>
        /// Returns spots for same sponsors as input spot
        /// </summary>
        /// <param name="spots"></param>
        /// <param name="spotInfos"></param>
        /// <returns></returns>
        public IReadOnlyCollection<Spot> GetSponsorSpots(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos
            )
        {
            return SpotUtilities.GetSpotsWithSameSponsor(
                spots,
                SmoothSpots.ConvertAll(s => s.Spot),
                spotInfos);
        }

        /// <summary>
        /// Returns list of spot positions that are filled.
        /// </summary>
        public IReadOnlyDictionary<string, bool> GetSpotPositions()
        {
            var spotPositions = new Dictionary<string, bool>();

            // Determine if we have any booked spots for ordinal positions (E.g.
            // 1st, 2nd, 3rd). These spots can't be moved. We can't really do
            // any checks for positions relative to last break as we don't know
            // how many spots that there should be.
            bool[] hasBookedSpotForPosition = new bool[4];
            for (int index = 0; index < hasBookedSpotForPosition.Length; index++)
            {
                string actualPosition = (index + 1).ToString();

                hasBookedSpotForPosition[index] = SmoothSpots.Any(s =>
                    !s.IsCurrent && s.Spot.ActualPositioninBreak == actualPosition);
            }

            // Check other spot positions. We don't check
            // Spot.RequestedPositionInBreak because the spot may not have been
            // placed in the break due to the particular PIB request.
            var hasSpotsForPositionInBreakRequest = new Dictionary<string, bool>();
            foreach (string currentPositionInBreakRequest in PositionInBreakRequests.All)
            {
                hasSpotsForPositionInBreakRequest[currentPositionInBreakRequest] = SmoothSpots
                    .Any(s => s.BreakSequence == SpotPositioning.GetBreakSeqFromRequestedPositionInBreak(currentPositionInBreakRequest));
            }

            spotPositions.Add("1ST_START", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.First] || hasSpotsForPositionInBreakRequest[PositionInBreakRequests.TrueFirst] || hasBookedSpotForPosition[0]);
            spotPositions.Add("2ND_START", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.SecondFromStart] || hasBookedSpotForPosition[1]);
            spotPositions.Add("3RD_START", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.ThirdFromStart] || hasBookedSpotForPosition[2]);
            spotPositions.Add("4TH_START", hasBookedSpotForPosition[3]);
            spotPositions.Add("3RD_LAST", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.ThirdFromLast]);
            spotPositions.Add("2ND_LAST", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.SecondFromLast]);
            spotPositions.Add("LAST", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.Last] || hasSpotsForPositionInBreakRequest[PositionInBreakRequests.TrueLast]);

            // Check for Multipart Top/Tail spots
            spotPositions.Add("TT|TOP",
                SmoothSpots.Any(s => s.BreakSequence == _breakSequenceForMultipartTopTailAndTopTailTop));

            spotPositions.Add("TT|TAIL",
                SmoothSpots.Any(s => s.BreakSequence == _breakSequenceForMultipartTopTailAndTopTailTail));

            // Check for Multipart Same Break spots
            spotPositions.Add("SB|TOP",
                SmoothSpots.Any(s => s.BreakSequence == _breakSequenceForMultipartSameBreakAndSameBreakTop));

            spotPositions.Add("SB|TAIL",
                SmoothSpots.Any(s => s.BreakSequence == _breakSequenceForMultipartSameBreakAndSameBreakTail));

            spotPositions.Add("SB|MID",
                SmoothSpots.Any(s => s.BreakSequence == _breakSequenceForMultipartSameBreakAndSameBreakMiddle));

            spotPositions.Add("SB|ANY",
                SmoothSpots.Any(s => s.BreakSequence == _breakSequenceForMultipartSameBreakAndSameBreakAny));

            return spotPositions;
        }

        /// <summary>
        /// Adds spot to break with specified break sequence. Spot may have been
        /// added in this Smooth run or a previous run.
        /// </summary>
        /// <param name="spot"></param>
        /// <param name="smoothPassSequence"></param>
        /// <param name="breakSeq"></param>
        /// <param name="currentSpot"></param>
        /// <param name="canMoveSpotToOtherBreak"></param>
        public SmoothSpot AddSpot(
            Spot spot,
            int smoothPassSequence,
            int smoothPassIterationSequence,
            int breakSeq,
            bool currentSpot,
            bool canMoveSpotToOtherBreak,
            string bestBreakFactorGroupName,
            string externalBreakRefAtStart)
        {
            if (currentSpot)
            {
                spot.ExternalBreakNo = TheBreak.ExternalBreakRef;
            }

            var smoothSpot = new SmoothSpot(spot, smoothPassSequence, smoothPassIterationSequence)
            {
                BreakSequence = breakSeq,
                IsCurrent = currentSpot,
                CanMoveToOtherBreak = canMoveSpotToOtherBreak,
                BestBreakFactorGroupName = bestBreakFactorGroupName,
                ExternalBreakRefAtStart = externalBreakRefAtStart
            };

            // Insert in ordinal position
            bool added = false;
            for (int spotIndex = 0; spotIndex < SmoothSpots.Count; spotIndex++)
            {
                // Insert before this one
                if (breakSeq < SmoothSpots[spotIndex].BreakSequence)
                {
                    SmoothSpots.Insert(spotIndex, smoothSpot);
                    added = true;

                    break;
                }
            }

            if (!added)
            {
                SmoothSpots.Add(smoothSpot);
            }

            RemainingAvailability -= spot.SpotLength;

            return smoothSpot;
        }

        /// <summary>
        /// Removes spot from break
        /// </summary>
        /// <param name="spot"></param>
        public void RemoveSpot(Spot spot)
        {
            spot.ExternalBreakNo = null;
            spot.ActualPositioninBreak = null;

            Guid spotUid = spot.Uid;
            SmoothSpot smoothSpot = SmoothSpots.Find(s => s.Spot.Uid == spotUid);
            _ = SmoothSpots.Remove(smoothSpot);

            RemainingAvailability += spot.SpotLength;
        }

        /// <summary>
        /// Renumbers actual position of the spot in break to reflect ordinal
        /// position of spots. This is necessary because during smoothing we may
        /// set large gaps of sequences so that we can insert spots in the gaps.
        /// </summary>
        public void RenumberActualPositionOfSpotInBreak()
        {
            // Get BreakSeqType for Same Break Mid
            var breakSeqTypeForMultipartSameBreakMid = SpotPositioning.GetBreakSeqTypeForMultipartSpot(
                MultipartSpotTypes.SameBreak,
                MultipartSpotPositions.SameBreak_Mid);

            // Get all spots except Same Break Mid multipart spot, they need to
            // be placed in the middle of the break
            var mainSpotsInBreakSequenceOrder = SmoothSpots
                .Where(s => s.BreakSequence < breakSeqTypeForMultipartSameBreakMid.MinSequence || s.BreakSequence > breakSeqTypeForMultipartSameBreakMid.MaxSequence)
                .OrderBy(s => s.BreakSequence)
                .ToList();

            // Place the Same Break Mid multipart spot(s) in the middle, should
            // only be one but handle multiple. For odd number of spots then it
            // is placed in the exact middle. For even number of spots then in
            // either of the two middle spots.
            var multipartSameBreakMidSpots = SmoothSpots
                .Where(s => s.BreakSequence >= breakSeqTypeForMultipartSameBreakMid.MinSequence && s.BreakSequence <= breakSeqTypeForMultipartSameBreakMid.MaxSequence)
                .OrderBy(s => s.BreakSequence)
                .ToList();

            while (multipartSameBreakMidSpots.Count > 0)
            {
                var multipartSpot = multipartSameBreakMidSpots[0];
                _ = multipartSameBreakMidSpots.Remove(multipartSpot);

                int placeMultipartSpotIndex = mainSpotsInBreakSequenceOrder.Count / 2;
                mainSpotsInBreakSequenceOrder.Insert(placeMultipartSpotIndex, multipartSpot);
            }

            int actualPositionInBreak = 1;

            // Set Spot.ActualPositionInBreak. Spot.ActualPositioninBreak cannot
            // be modified for booked spots.
            foreach (SmoothSpot smoothSpot in mainSpotsInBreakSequenceOrder.Where(s => s.IsCurrent))
            {
                string position = actualPositionInBreak.ToString();

                // Ensure that break sequence is unique if already used by a
                // booked spot.
                while (SmoothSpots.Any(s => !s.IsCurrent && s.Spot.ActualPositioninBreak == position))
                {
                    actualPositionInBreak++;
                    position = actualPositionInBreak.ToString();
                }

                smoothSpot.Spot.ActualPositioninBreak = position;
                actualPositionInBreak++;
            }
        }
    }
}
