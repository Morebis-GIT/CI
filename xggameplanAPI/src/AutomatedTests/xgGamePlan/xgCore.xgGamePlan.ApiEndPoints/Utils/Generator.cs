using System;
using System.Collections.Generic;
using System.Globalization;

namespace xgCore.xgGamePlan.ApiEndPoints.Utils
{
    public static class Generator
    {
        private readonly static Random random = new Random();
        private const int MaximumTry = 1000000;
        public static List<int> GetUniqueNumbers(List<string> existingNumbers, int count)
        {
            var numbers = new List<int>();
            int c = 0;
            while (c < MaximumTry)
            {
                c++;
                int nextNumber = random.Next();
                if (!numbers.Contains(nextNumber) && !existingNumbers.Contains(nextNumber.ToString(CultureInfo.InvariantCulture)))
                {
                    numbers.Add(nextNumber);
                    if (numbers.Count == count)
                    {
                        return numbers;
                    }
                }
            }
            throw new Exception("Cannot get GetUniqueNumbers.");
        }
    }
}
