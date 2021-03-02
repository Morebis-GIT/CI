using System;
using System.Collections.Generic;
using System.Linq;
using xggameplan.common;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Manages spot position by generating the break sequence number. The break
    /// sequence numbers are eventually converted back to ordinal positions
    /// after all spots have been placed.
    /// </summary>
    public class SpotPositioning
    {
        public class BreakSeqType
        {
            /// <summary>
            /// Categories of break sequence type
            /// </summary>
            public enum Categories
            {
                /// <summary>
                /// Requested position in break
                /// </summary>
                RequestedPositionInBreak = 0,

                /// <summary>
                /// Multipart spot
                /// </summary>
                MultipartSpot = 1,

                /// <summary>
                /// Middle of break, spots that don't have a requested position
                /// in break
                /// </summary>
                MiddleOfBreak = 2
            }

            public int Order { get; internal set; }
            public Categories Category { get; internal set; }
            public string RequestedPositionInBreak { get; internal set; }
            public string MultipartSpotType { get; set; }

            /// <summary>
            /// Min sequence number
            /// </summary>
            public int MinSequence { get; internal set; }

            /// <summary>
            /// Max sequence number, typically used for Categories.MiddleOfBreak
            /// where there are multiple spots, each with a different sequence
            /// </summary>
            public int MaxSequence { get; internal set; }

            public BreakSeqType(int order, Categories category, string requestPositionInBreak, string multipartSpotType, int minSequence)
            {
                Order = order;
                Category = category;
                RequestedPositionInBreak = requestPositionInBreak;
                MultipartSpotType = multipartSpotType;
                MinSequence = minSequence;
                MaxSequence = minSequence;
            }

            public BreakSeqType(int order, Categories category, string requestPositionInBreak, string multipartSpotType, int minSequence, int maxSequence)
            {
                Order = order;
                Category = category;
                RequestedPositionInBreak = requestPositionInBreak;
                MultipartSpotType = multipartSpotType;
                MinSequence = minSequence;
                MaxSequence = maxSequence;
            }
        }

        private readonly List<BreakSeqType> _breakSeqTypes;

        public SpotPositioning() => _breakSeqTypes = GetAll();

        private List<BreakSeqType> GetAll()
        {
            var breakSeqTypes = new List<BreakSeqType>();

            // First in break
            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(1, PositionInBreakRequests.TrueFirst, -150000));
            breakSeqTypes.Add(CreateRequestedPositionInBreakRequest(2, PositionInBreakRequests.First, -140000));

            // Multipart Top/Tail (Top)
            breakSeqTypes.Add(CreateMultipartSpot(3, MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.TopTail, MultipartSpotPositions.TopTail_Top), -130000));

            // Multipart Same Break (Top)
            breakSeqTypes.Add(CreateMultipartSpot(4, MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Top), -120000));

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

        public BreakSeqType GetBreakSeqTypeForMultipartSpot(string multipartSpot, string multipartSpotPosition)
        {
            return _breakSeqTypes.Find(bst =>
                bst.Category == BreakSeqType.Categories.MultipartSpot
                && bst.MultipartSpotType == MultipartSpotTypes.GetSpotTypeAndPositionKey(multipartSpot, multipartSpotPosition)
                );
        }

        public string GetRequestedPositionInBreakFromSpotPosition(string spotPosition, string first, string last)
        {
            var requestedPositionInBreaks = new Dictionary<string, string>()
            {
                { "1ST_START", first },
                { "2ND_START", PositionInBreakRequests.SecondFromStart },
                { "3RD_START", PositionInBreakRequests.ThirdFromStart },
                { "3RD_LAST", PositionInBreakRequests.ThirdFromLast },
                { "2ND_LAST", PositionInBreakRequests.SecondFromLast },
                { "LAST", last }
            };
            return requestedPositionInBreaks.ContainsKey(spotPosition) ? requestedPositionInBreaks[spotPosition] : "";
        }

        /// <summary>
        /// Returns default break sequence for the requested position in break
        /// </summary>
        /// <param name="requestedPositionInBreak"></param>
        /// <returns></returns>
        public int GetBreakSeqFromRequestedPositionInBreak(string requestedPositionInBreak)
        {
            var breakSeqType = _breakSeqTypes.Find(
                b =>
                b.Category == BreakSeqType.Categories.RequestedPositionInBreak &&
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
        public int GetBreakSeqFromRequestedPositionInBreakOrActualPositionInBreak(
            string requestedPositionInBreak,
            string actualPositionInBreak
            )
        {
            // Check if requested PIB set
            if (!String.IsNullOrEmpty(requestedPositionInBreak))
            {
                return GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);
            }

            // If requested PIB not set then check actual PIB. We do this rather
            // than placing the spot in the middle of the break because these
            // are effectively PIB requests anyway.
            switch (actualPositionInBreak)
            {
                case "1": return GetBreakSeqFromRequestedPositionInBreak(PositionInBreakRequests.First);
                case "2": return GetBreakSeqFromRequestedPositionInBreak(PositionInBreakRequests.SecondFromStart);
                case "3": return GetBreakSeqFromRequestedPositionInBreak(PositionInBreakRequests.ThirdFromStart);
            }

            // Middle of break, anything else (E.g. 4th, 5th etc
            return GetBreakSeqForMiddleOfBreak(Convert.ToInt32(actualPositionInBreak));
        }

        /// <summary>
        /// Returns default break sequence for putting the spot in the middle of
        /// the break with specified position.
        /// </summary>
        /// <param name="spots"></param>
        /// <returns></returns>
        private int GetBreakSeqForMiddleOfBreak(int position)
        {
            BreakSeqType breakSeqType = _breakSeqTypes.Find(b => b.Category == BreakSeqType.Categories.MiddleOfBreak);
            return breakSeqType.MinSequence + (position - 1);      // So that postion 1 is BreakSeqType.Position
        }

        /// <summary>
        /// Returns default break sequence for putting the spot in the middle of
        /// the break at the next free position
        /// </summary>
        /// <returns></returns>
        public int GetBreakSeqForMiddleOfBreak(SmoothBreak smoothBreak)
        {
            BreakSeqType breakSeqType = _breakSeqTypes.Find(b => b.Category == BreakSeqType.Categories.MiddleOfBreak);

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
        /// <returns></returns>
        public int GetBreakSeqFromMultipartSpotPosition(string multipartSpot, string multipartSpotPosition)
        {
            string multipartSpotType = MultipartSpotTypes.GetSpotTypeAndPositionKey(multipartSpot, multipartSpotPosition);
            BreakSeqType breakSeqType = _breakSeqTypes.Find(b => b.Category == BreakSeqType.Categories.MultipartSpot && b.MultipartSpotType == multipartSpotType);

            return breakSeqType?.MinSequence ?? 0;
        }

        private BreakSeqType CreateRequestedPositionInBreakRequest(int order, string requestedPositionInBreakRequest, int sequence)
        {
            return new BreakSeqType(order, BreakSeqType.Categories.RequestedPositionInBreak, requestedPositionInBreakRequest, null, sequence);
        }

        private BreakSeqType CreateMultipartSpot(int order, string multipartSpotType, int sequence)
        {
            return new BreakSeqType(order, BreakSeqType.Categories.MultipartSpot, null, multipartSpotType, sequence);
        }

        private BreakSeqType CreateMiddleOfBreak(int order, int minSequence, int maxSequence)
        {
            return new BreakSeqType(order, BreakSeqType.Categories.MiddleOfBreak, null, null, minSequence, maxSequence);
        }
    }
}
