using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.common;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Manages spot position by generating the break sequence number. The break
    /// sequence numbers are eventually converted back to ordinal positions
    /// after all spots have been placed.
    /// </summary>
    public static class SpotPositioning
    {
        // Value given when a break sequence number cannot be found.
        private const int ZeroBreakSequenceNumber = 0;

        private static readonly List<BreakSequenceType> _breakSeqTypes;

        static SpotPositioning() => _breakSeqTypes = GetAll();

        private static List<BreakSequenceType> GetAll()
        {
            var breakSeqTypes = new List<BreakSequenceType>();

            // First in break
            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(1, PositionInBreakRequests.TrueFirst, -150000));
            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(2, PositionInBreakRequests.First, -140000));

            // Multipart Top/Tail (Top)
            breakSeqTypes.Add(
                CreateMultipartSpot(
                    3,
                    MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.TopTail, MultipartSpotPositions.TopTail_Top),
                    -130000)
                );

            // Multipart Same Break (Top)
            breakSeqTypes.Add(
                CreateMultipartSpot(
                    4,
                    MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Top),
                    -120000)
                );

            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(5, PositionInBreakRequests.SecondFromStart, -110000));
            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(6, PositionInBreakRequests.ThirdFromStart, -100000));

            // Middle of break, from 4th onwards
            breakSeqTypes.Add(CreateMiddleOfBreak(7, 1, 10000));

            // Multipart Same Break (Mid)
            breakSeqTypes.Add(CreateMultipartSpot(8, MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Mid), 20000));

            // Multipart Same Break (Any)
            breakSeqTypes.Add(CreateMultipartSpot(9, MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Any), 30000));

            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(10, PositionInBreakRequests.ThirdFromLast, 100000));
            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(11, PositionInBreakRequests.SecondFromLast, 110000));

            // Multipart Same Break (Tail)
            breakSeqTypes.Add(CreateMultipartSpot(12, MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Tail), 120000));

            // Multipart Top/Tail (Tail)
            breakSeqTypes.Add(CreateMultipartSpot(13, MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.TopTail, MultipartSpotPositions.TopTail_Tail), 130000));

            // Last in break
            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(14, PositionInBreakRequests.Last, 140_000));
            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(15, PositionInBreakRequests.TrueLast, 150_000));

            return breakSeqTypes;
        }

        public static BreakSequenceType GetBreakSeqTypeForMultipartSpot(
            string multipartSpot,
            string multipartSpotPosition)
        {
            string multipartSpotType = MultipartSpotTypes.GetSpotTypeAndPositionKey(
                multipartSpot,
                multipartSpotPosition);

            return _breakSeqTypes.Find(bst =>
                bst.Category == BreakSequenceTypeCategory.MultipartSpot &&
                bst.MultipartSpotType == multipartSpotType
            );
        }

        /// <summary>
        /// Returns default break sequence for the requested position in break
        /// </summary>
        /// <param name="requestedPositionInBreak"></param>
        /// <returns></returns>
        public static int GetBreakSeqFromRequestedPositionInBreak(
            string requestedPositionInBreak)
        {
            var breakSeqType = _breakSeqTypes.Find(b =>
                b.Category == BreakSequenceTypeCategory.RequestedPositionInBreak &&
                b.RequestedPositionInBreak == requestedPositionInBreak
            );

            return breakSeqType?.MinSequence ?? 0;
        }

        /// <summary>
        /// Returns default break sequence for the requested position in
        /// break/actual position in break, typically for a booked spot
        /// </summary>
        /// <param name="requestedPositionInBreak"></param>
        /// <param name="actualPositionInBreak"></param>
        /// <returns></returns>
        public static int GetBreakSeqFromRequestedPositionInBreakOrActualPositionInBreak(
            string requestedPositionInBreak,
            string actualPositionInBreak)
        {
            // Check if requested PIB set
            if (!String.IsNullOrEmpty(requestedPositionInBreak))
            {
                return GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);
            }

            // If requested PIB not set then check actual PIB. We do this rather
            // than placing the spot in the middle of the break because these
            // are effectively PIB requests anyway.
            return actualPositionInBreak switch
            {
                "1" => GetBreakSeqFromRequestedPositionInBreak(PositionInBreakRequests.First),
                "2" => GetBreakSeqFromRequestedPositionInBreak(PositionInBreakRequests.SecondFromStart),
                "3" => GetBreakSeqFromRequestedPositionInBreak(PositionInBreakRequests.ThirdFromStart),
                // Middle of break, anything else (E.g. 4th, 5th etc
                _ => GetBreakSeqForMiddleOfBreak(Convert.ToInt32(actualPositionInBreak)),
            };
        }

        /// <summary>
        /// Returns default break sequence for putting the spot in the middle of
        /// the break with specified position.
        /// </summary>
        private static int GetBreakSeqForMiddleOfBreak(int position)
        {
            BreakSequenceType breakSeqType = _breakSeqTypes.Find(b =>
            b.Category == BreakSequenceTypeCategory.MiddleOfBreak);

            // So that position 1 is BreakSeqType.Position
            return breakSeqType.MinSequence + (position - 1);
        }

        /// <summary>
        /// Returns default break sequence for putting the spot in the middle of
        /// the break at the next free position
        /// </summary>
        /// <returns></returns>
        public static int GetBreakSeqForMiddleOfBreak(SmoothBreak smoothBreak)
        {
            BreakSequenceType breakSeqType = _breakSeqTypes.Find(b =>
                b.Category == BreakSequenceTypeCategory.MiddleOfBreak);

            int position = 3;       // After 3rd

            while (true)
            {
                // Check if break sequence is assigned
                position++;
                int breakSeq = GetBreakSeqForMiddleOfBreak(position);
                var spots = smoothBreak.SmoothSpots.Where(s => s.BreakSequence == breakSeq);

                if (!spots.Any())
                {
                    return breakSeq;
                }
            }
        }

        /// <summary>
        /// Returns default break sequence for the multipart spot
        /// </summary>
        /// <param name="multipartSpot"></param>
        /// <param name="multipartSpotPosition"></param>
        public static int GetBreakSeqFromMultipartSpotPosition(
            string multipartSpot,
            string multipartSpotPosition)
        {
            string multipartSpotType = MultipartSpotTypes.GetSpotTypeAndPositionKey(
                multipartSpot,
                multipartSpotPosition);

            BreakSequenceType breakSeqType = _breakSeqTypes.Find(b =>
                b.Category == BreakSequenceTypeCategory.MultipartSpot &&
                b.MultipartSpotType == multipartSpotType);

            return breakSeqType?.MinSequence ?? 0;
        }

        private static BreakSequenceType CreateRequestedPositionInBreakRequest(
            int order,
            string requestedPositionInBreakRequest,
            int sequence)
        {
            return new BreakSequenceType(
                order,
                BreakSequenceTypeCategory.RequestedPositionInBreak,
                requestedPositionInBreakRequest,
                null,
                sequence);
        }

        private static BreakSequenceType CreateMultipartSpot(
            int order,
            string multipartSpotType,
            int sequence)
        {
            return new BreakSequenceType(
                order,
                BreakSequenceTypeCategory.MultipartSpot,
                null,
                multipartSpotType,
                sequence);
        }

        private static BreakSequenceType CreateMiddleOfBreak(
            int order,
            int minSequence,
            int maxSequence)
        {
            return new BreakSequenceType(
                order,
                BreakSequenceTypeCategory.MiddleOfBreak,
                null,
                null,
                minSequence,
                maxSequence);
        }

        public static int GetBreakSequenceNumber(
            SmoothBreak smoothBreak,
            SpotPositionRules passRequestedPositionInBreakRules,
            Spot spot,
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            return passRequestedPositionInBreakRules switch
            {
                SpotPositionRules.Exact => ExactBreakSequenceNumber(smoothBreak, spot),
                SpotPositionRules.Near => NearBreakSequenceNumber(smoothBreak, spot, hasSpotPositions),
                SpotPositionRules.Anywhere => GetBreakSeqForMiddleOfBreak(smoothBreak),
                _ => 0,
            };
        }

        private static int ExactBreakSequenceNumber(
            SmoothBreak smoothBreak,
            Spot spot)
        {
            if (spot.IsMultipartSpot)
            {
                return GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
            }
            else
            {
                return String.IsNullOrEmpty(spot.RequestedPositioninBreak) ?
                    GetBreakSeqForMiddleOfBreak(smoothBreak) :
                    GetBreakSeqFromRequestedPositionInBreak(spot.RequestedPositioninBreak);
            }
        }

        private static int NearBreakSequenceNumber(
            SmoothBreak smoothBreak,
            Spot spot,
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            return spot.IsMultipartSpot
                ? NearBreakSequenceNumberForMultipartSpot(smoothBreak, spot, hasSpotPositions)
                : NearBreakSequenceNumberForRegularSpot(smoothBreak, spot, hasSpotPositions);
        }

        private static int NearBreakSequenceNumberForMultipartSpot(
            SmoothBreak smoothBreak,
            Spot spot,
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            int breakSeq = 0;

            switch (spot.MultipartSpot)
            {
                case MultipartSpotTypes.TopTail:
                    breakSeq = BreakSequenceNumberForMultipartTopTailSpot(
                        spot,
                        hasSpotPositions,
                        breakSeq);

                    break;

                case MultipartSpotTypes.SameBreak:
                    breakSeq = BreakSequenceNumberForMultipartSameBreakSpot(
                        spot,
                        hasSpotPositions,
                        breakSeq);

                    break;
            }

            // Default to middle of break
            if (breakSeq == 0)
            {
                breakSeq = GetBreakSeqForMiddleOfBreak(smoothBreak);
            }

            return breakSeq;
        }

        private static int BreakSequenceNumberForMultipartSameBreakSpot(
            Spot spot,
            IReadOnlyDictionary<string, bool> hasSpotPositions,
            int breakSeq)
        {
            switch (spot.MultipartSpotPosition)
            {
                case MultipartSpotPositions.SameBreak_Top:
                    if (!hasSpotPositions["1ST_START"])
                    {
                        breakSeq = GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                    }

                    break;

                case MultipartSpotPositions.SameBreak_Mid:
                    breakSeq = GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);

                    break;

                case MultipartSpotPositions.SameBreak_Tail:
                    if (!hasSpotPositions["LAST"])
                    {
                        breakSeq = GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                    }

                    break;

                case MultipartSpotPositions.SameBreak_Any:
                    breakSeq = GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                    break;
            }

            return breakSeq;
        }

        private static int BreakSequenceNumberForMultipartTopTailSpot(
            Spot spot,
            IReadOnlyDictionary<string, bool> hasSpotPositions,
            int breakSeq)
        {
            switch (spot.MultipartSpotPosition)
            {
                case MultipartSpotPositions.TopTail_Top:
                    if (!hasSpotPositions["1ST_START"])
                    {
                        breakSeq = GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                    }

                    break;

                case MultipartSpotPositions.TopTail_Tail:
                    if (!hasSpotPositions["LAST"])
                    {
                        breakSeq = GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                    }

                    break;
            }

            return breakSeq;
        }

        private static int NearBreakSequenceNumberForRegularSpot(
            SmoothBreak smoothBreak,
            Spot spot,
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            return spot.RequestedPositioninBreak switch
            {
                PositionInBreakRequests.TrueFirst => BreakSequenceNumberForRegularTrueFirstSpot(hasSpotPositions),
                PositionInBreakRequests.First => BreakSequenceNumberForRegularFirstSpot(hasSpotPositions),
                PositionInBreakRequests.SecondFromStart => BreakSequenceNumberForRegularSecondFromStartSpot(hasSpotPositions),
                PositionInBreakRequests.ThirdFromStart => BreakSequenceNumberForRegularThirdFromStartSpot(hasSpotPositions),
                PositionInBreakRequests.ThirdFromLast => BreakSequenceNumberForRegularThirdFromLastSpot(hasSpotPositions),
                PositionInBreakRequests.SecondFromLast => BreakSequenceNumberForRegularSecondFromLastSpot(hasSpotPositions),
                PositionInBreakRequests.Last => BreakSequenceNumberForRegularLastSpot(hasSpotPositions),
                PositionInBreakRequests.TrueLast => BreakSequenceNumberForRegularTrueLastSpot(hasSpotPositions),

                // No PIB request
                _ => GetBreakSeqForMiddleOfBreak(smoothBreak)
            };
        }

        private static string GetRequestedPositionInBreakFromSpotPosition(
            string spotPosition,
            string first,
            string last)
        {
            return spotPosition switch
            {
                "1ST_START" => first,
                "2ND_START" => PositionInBreakRequests.SecondFromStart,
                "3RD_START" => PositionInBreakRequests.ThirdFromStart,
                "3RD_LAST" => PositionInBreakRequests.ThirdFromLast,
                "2ND_LAST" => PositionInBreakRequests.SecondFromLast,
                "LAST" => last,
                _ => String.Empty
            };
        }

        private static int GetBreakSequenceNumberFromSpotPositions(
            string[] spotPositions,
            IReadOnlyDictionary<string, bool> hasSpotPositions,
            (string first, string last) pibRequests)
        {
            foreach (string spotPosition in spotPositions)
            {
                if (hasSpotPositions[spotPosition])
                {
                    continue;
                }

                string requestedPositionInBreak = GetRequestedPositionInBreakFromSpotPosition(
                    spotPosition,
                    pibRequests.first,
                    pibRequests.last);

                return GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);
            }

            return ZeroBreakSequenceNumber;
        }

        private static int BreakSequenceNumberForRegularTrueFirstSpot(
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            // Check existing PIB requests in priority
            // order from most to least desired
            string[] spotPositions =
                hasSpotPositions["1ST_START"] ||
                hasSpotPositions["TT|TOP"] ||
                hasSpotPositions["SB|TOP"]
                ? new[] { "2ND_START", "3RD_START" }
                : new[] { "1ST_START", "2ND_START", "3RD_START" };

            return GetBreakSequenceNumberFromSpotPositions(
                spotPositions,
                hasSpotPositions,
                (PositionInBreakRequests.TrueFirst, PositionInBreakRequests.TrueLast)
                );
        }

        private static int BreakSequenceNumberForRegularFirstSpot(
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            // Check existing PIB requests in priority
            // order from most to least desired
            string[] spotPositions =
                hasSpotPositions["1ST_START"] ||
                hasSpotPositions["TT|TOP"] ||
                hasSpotPositions["SB|TOP"]
                ? new[] { "2ND_START", "3RD_START" }
                : new[] { "1ST_START", "2ND_START", "3RD_START" };

            return GetBreakSequenceNumberFromSpotPositions(
                spotPositions,
                hasSpotPositions,
                (PositionInBreakRequests.First, PositionInBreakRequests.Last)
                );
        }

        private static int BreakSequenceNumberForRegularSecondFromStartSpot(
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            // Check existing PIB requests in priority
            // order from most to least desired
            string[] spotPositions = hasSpotPositions["2ND_START"]
                ? new[] { "3RD_START" }
                : new[] { "2ND_START", "3RD_START" };

            return GetBreakSequenceNumberFromSpotPositions(
                spotPositions,
                hasSpotPositions,
                (PositionInBreakRequests.First, PositionInBreakRequests.Last)
                );
        }

        private static int BreakSequenceNumberForRegularThirdFromStartSpot(
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            // Check existing PIB requests in priority
            // order from most to least desired
            string[] spotPositions = hasSpotPositions["3RD_START"]
                ? new[] { "2ND_START" }
                : new[] { "3RD_START", "2ND_START" };

            return GetBreakSequenceNumberFromSpotPositions(
                spotPositions,
                hasSpotPositions,
                (PositionInBreakRequests.First, PositionInBreakRequests.Last)
                );
        }

        private static int BreakSequenceNumberForRegularThirdFromLastSpot(
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            // Check existing PIB requests in priority
            // order from most to least desired
            return GetBreakSequenceNumberFromSpotPositions(
                new[] { "3RD_LAST", "2ND_LAST" },
                hasSpotPositions,
                (PositionInBreakRequests.First, PositionInBreakRequests.Last)
                );
        }

        private static int BreakSequenceNumberForRegularSecondFromLastSpot(
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            // Check existing PIB requests in priority
            // order from most to least desired
            return GetBreakSequenceNumberFromSpotPositions(
                new[] { "2ND_LAST", "3RD_LAST" },
                hasSpotPositions,
                (PositionInBreakRequests.First, PositionInBreakRequests.Last)
                );
        }

        private static int BreakSequenceNumberForRegularLastSpot(
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            // Check existing PIB requests in priority
            // order from most to least desired
            string[] spotPositions =
                hasSpotPositions["LAST"] ||
                hasSpotPositions["TT|TAIL"] ||
                hasSpotPositions["SB|TAIL"]
                ? new[] { "2ND_LAST", "3RD_LAST" }
                : new[] { "LAST", "2ND_LAST", "3RD_LAST" };

            return GetBreakSequenceNumberFromSpotPositions(
                spotPositions,
                hasSpotPositions,
                (PositionInBreakRequests.First, PositionInBreakRequests.Last)
                );
        }

        private static int BreakSequenceNumberForRegularTrueLastSpot(
            IReadOnlyDictionary<string, bool> hasSpotPositions)
        {
            // Check existing PIB requests in priority
            // order from most to least desired
            string[] spotPositions =
                hasSpotPositions["LAST"] ||
                hasSpotPositions["TT|TAIL"] ||
                hasSpotPositions["SB|TAIL"]
                ? new[] { "2ND_LAST", "3RD_LAST" }
                : new[] { "LAST", "2ND_LAST", "3RD_LAST" };

            return GetBreakSequenceNumberFromSpotPositions(
                spotPositions,
                hasSpotPositions,
                (PositionInBreakRequests.TrueFirst, PositionInBreakRequests.TrueLast)
                );
        }
    }
}
