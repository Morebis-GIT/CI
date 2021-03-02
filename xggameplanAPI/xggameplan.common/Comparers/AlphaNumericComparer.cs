using System;
using System.Collections.Generic;
using System.Numerics;

namespace xggameplan.Common
{
    public class AlphaNumericComparer : IComparer<object>
    {
        /// <summary>Compares two object values. Digits in the strings are
        /// considered as numerical content rather than text. This test is not
        /// case-sensitive.</summary>
        /// <param name="x">The first object to be compared.</param>
        /// <param name="y">The second object to be compared.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item>Returns zero if the strings are identical.</item>
        /// <item>Returns 1 if the string pointed to by x has a greater value than that pointed to by y.</item>
        /// <item>Returns -1 if the string pointed to by x has a lesser value than that pointed to by y.</item>
        /// </list>
        /// </returns>
        /// <remarks>The comparison is similar to StrCmpLogicalW function (shlwapi.h)</remarks>
        public int Compare(object x, object y)
        {
            if (x == y)
            {
                return 0;
            }

            if (!(x is string sx))
            {
                return -1;
            }

            if (!(y is string sy))
            {
                return 1;
            }

            int sxLength = sx.Length;
            int syLength = sy.Length;
            int sxIndex = 0;
            int syIndex = 0;

            // Go through each string by indexes.
            while (sxIndex < sxLength && syIndex < syLength)
            {
                // Find the characters that are digits or characters in BOTH
                // strings starting at the appropriate marker and save them into
                // char arrays.
                string str1;
                string str2;

                (sxIndex, str1) = NextCharacterBlock(sx, sxIndex);
                (syIndex, str2) = NextCharacterBlock(sy, syIndex);

                // If numbers then compare numerically else if strings compare alphabetically.
                const int NoResult = -2;
                int result = NoResult;

                if (Char.IsDigit(str1[0]) && Char.IsDigit(str2[0]))
                {
                    bool containsNumber = BigInteger.TryParse(str1, out var thisNumericChunk);
                    containsNumber &= BigInteger.TryParse(str2, out var thatNumericChunk);

                    if (containsNumber)
                    {
                        result = thisNumericChunk.CompareTo(thatNumericChunk);
                    }
                }

                if (result == NoResult)
                {
                    result = str1.CompareTo(str2);
                }

                if (result != 0)
                {
                    return result;
                }
            }

            return Math.Min(1, Math.Max(-1, sxLength - syLength));

            //---------------
            // Local function
            (int index, string codeBlock) NextCharacterBlock(
                in string source,
                in int nextCharacterIndex)
            {
                int sourceLength = source.Length;
                int index = nextCharacterIndex;
                int loc_x = 0;
                bool isDigitBlockX = Char.IsDigit(source[index]);

                do
                {
                    loc_x++;
                    index++;

                    if (index >= sourceLength)
                    {
                        break;
                    }
                } while (Char.IsDigit(source[index]) == isDigitBlockX);

                var codeBlock = source.Substring(nextCharacterIndex, loc_x);

                return (index, codeBlock);
            }
        }
    }
}
