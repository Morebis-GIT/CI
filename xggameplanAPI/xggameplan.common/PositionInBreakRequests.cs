using System;
using System.Collections.Generic;

namespace xggameplan.common
{
    public static class PositionInBreakRequests
    {
        /// <summary>
        /// True First in Break (Guaranteed)
        /// </summary>
        public const string TrueFirst = "TRF";

        /// <summary>
        /// Requested First in Break
        /// </summary>
        public const string First = "FIB";

        /// <summary>
        /// Requested Second in Break
        /// </summary>
        public const string SecondFromStart = "2ND";

        /// <summary>
        /// Requested Third in Break
        /// </summary>
        public const string ThirdFromStart = "3RD";

        /// <summary>
        /// Requested Third from Last
        /// </summary>
        public const string ThirdFromLast = "3LS";

        /// <summary>
        /// Requested Second from Last
        /// </summary>
        public const string SecondFromLast = "2LS";

        /// <summary>
        /// Requested Last in Break
        /// </summary>
        public const string Last = "LIB";

        /// <summary>
        /// True Last in Break (Guaranteed)
        /// </summary>
        public const string TrueLast = "TRL";

        /// <summary>
        /// Returns all position in break requests for the same position.
        /// </summary>
        /// <param name="positionInBreakRequest"></param>
        /// <returns></returns>
        public static List<string> GetPositionInBreakRequestsForSamePosition(
            string positionInBreakRequest)
        {
            var positionInBreakRequests = new List<string>();

            foreach (string[] pair in FirstAndLast)
            {
                if (Array.IndexOf(pair, positionInBreakRequest) != -1)
                {
                    positionInBreakRequests.AddRange(pair);
                    break;
                }
            }

            if (positionInBreakRequests.Count == 0)
            {
                positionInBreakRequests.Add(positionInBreakRequest);
            }

            return positionInBreakRequests;
        }

        public static List<string> All =>
            new List<string>()
            {
                TrueFirst,
                First,
                SecondFromStart,
                SecondFromLast,
                ThirdFromStart,
                ThirdFromLast,
                Last,
                TrueLast
            };

        private static List<string[]> FirstAndLast =>
            new List<string[]>()
            {
                new string[] { TrueFirst, First },
                new string[] { TrueLast, Last }
            };
    }
}
