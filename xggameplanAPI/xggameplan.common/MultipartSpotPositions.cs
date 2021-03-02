using System.Collections.Generic;

namespace xggameplan.Common
{
    public static class MultipartSpotPositions
    {
        public const string TopTail_Top = "TOP";
        public const string TopTail_Tail = "TAIL";

        public const string SameBreak_Top = "TOP";
        public const string SameBreak_Tail = "TAIL";
        public const string SameBreak_Mid = "MID";
        public const string SameBreak_Any = "ANY";

        public const string End = "END";

        public static List<string> All
        {
            get
            {
                return new List<string>() {
                    TopTail_Top,
                    TopTail_Tail,
                    SameBreak_Top,
                    SameBreak_Tail,
                    SameBreak_Mid,
                    SameBreak_Any
                };
            }
        }

        /// <summary>
        /// Returns all spot positions for specific multipart spot type
        /// </summary>
        /// <param name="multipartSpotType"></param>
        /// <returns></returns>
        public static List<string> GetAllForType(string multipartSpotType)
        {
            switch (multipartSpotType)
            {
                case MultipartSpotTypes.TopTail:
                    return new List<string>() { TopTail_Top, TopTail_Tail };

                case MultipartSpotTypes.SameBreak:
                    return new List<string>() { SameBreak_Top, SameBreak_Tail, SameBreak_Mid, SameBreak_Any };
            }
            return new List<string>();
        }

        /// <summary>
        /// For Same Break spots then returns the mid indexes for the number of spots
        /// </summary>
        /// <param name="numberOfSpots"></param>
        /// <returns></returns>
        public static int[] GetSameBreakMidIndexes(int numberOfSpots)
        {
            if (numberOfSpots % 2 == 1)   // Odd number of spots, single mid position
            {
                return new int[] { numberOfSpots / 2 };
            }
            else       // Even number of spots, 2 mid positions
            {
                return new int[] { (numberOfSpots / 2) - 1, numberOfSpots / 2 };
            }
        }
    }
}
