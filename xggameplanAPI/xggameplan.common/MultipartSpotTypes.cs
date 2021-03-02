using System.Collections.Generic;

namespace xggameplan.Common
{
    /// <summary>
    /// Multipart spot types
    /// </summary>
    public static class MultipartSpotTypes
    {
        public const string TopTail = "TT";
        public const string SameBreak = "SB";

        public static List<string> All =>
            new List<string>() { TopTail, SameBreak };

        public static string GetSpotTypeAndPositionKey(string multipartSpot, string multipartSpotPosition) =>
            $"{multipartSpot}|{multipartSpotPosition}";
    }
}
